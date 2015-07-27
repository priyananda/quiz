using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using RK.Common.GraphicsEngine.Core;

using D3D11 = SharpDX.Direct3D11;

namespace RK.Common.GraphicsEngine.Drawing3D
{
    public class SceneLayer
    {
        //All collections needed to link all scene objects to corresponding render passes
        private Dictionary<RenderPassInfo, List<RenderPassSubscription>> m_objectsPassCollections;
        private List<RenderPassSubscription> m_objectsPassPlainRender;
        private List<RenderPassSubscription> m_objectsPassTransparentRender;
        private RenderPassDefaultTransparent m_renderPassTransparent;

        //All generic members
        private List<SceneObject> m_sceneObjects;
        private ReadOnlyCollection<SceneObject> m_sceneObjectsPublic;
        private Scene m_scene;
        private string m_name;

        //public event Rendering3DHandler BeginRender;
        //public event Rendering3DHandler FinishRender;

        /// <summary>
        /// Creates a new SceneLayer object for the given scene.
        /// </summary>
        /// <param name="parentScene">Parent scene.</param>
        internal SceneLayer(string name, Scene parentScene)
        {
            m_name = name;
            m_scene = parentScene;

            //Create all specialized render pass lists
            m_objectsPassPlainRender = new List<RenderPassSubscription>(1024);
            m_objectsPassTransparentRender = new List<RenderPassSubscription>(1024);
            
            //Create dictionary for fast access to all render pass list
            m_objectsPassCollections = new Dictionary<RenderPassInfo, List<RenderPassSubscription>>();
            m_objectsPassCollections[RenderPassInfo.PASS_PLAIN_RENDER] = m_objectsPassPlainRender;
            m_objectsPassCollections[RenderPassInfo.PASS_TRANSPARENT_RENDER] = m_objectsPassTransparentRender;

            m_renderPassTransparent = new RenderPassDefaultTransparent(RenderPassInfo.PASS_TRANSPARENT_RENDER.Name);

            //Create standard collections
            m_sceneObjects = new List<SceneObject>();
            m_sceneObjectsPublic = new ReadOnlyCollection<SceneObject>(m_sceneObjects);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Layer " + m_name;
        }

        /// <summary>
        /// Subscribes the given object to the given render pass.
        /// </summary>
        /// <param name="passInfo">The pass to subscribe on.</param>
        /// <param name="sceneObject">The scene object to subscribe.</param>
        internal RenderPassSubscription SubscribeForPass(RenderPassInfo passInfo, SceneObject sceneObject, Action<RenderState> renderMethod)
        {
            var newSubscription = new RenderPassSubscription(this, passInfo, sceneObject, renderMethod);
            m_objectsPassCollections[passInfo].Add(newSubscription);
            return newSubscription;
        }

        /// <summary>
        /// Unsubscribes the given object from the given render pass.
        /// </summary>
        /// <param name="passInfo">The pass to unsubscribe from.</param>
        /// <param name="sceneObject">The scene object to unsubscribe.</param>
        internal void UnsubscribeForPass(RenderPassInfo passInfo, SceneObject sceneObject)
        {
            List<RenderPassSubscription> subscriptionList = m_objectsPassCollections[passInfo];
            for (int loop = 0; loop < subscriptionList.Count; loop++)
            {
                if (subscriptionList[loop].SceneObject == sceneObject)
                {
                    subscriptionList.RemoveAt(loop);
                    return;
                }
            }
        }

        /// <summary>
        /// Adds the given object to the layer.
        /// </summary>
        /// <param name="sceneObject">Object to add.</param>
        public void Add(SceneObject sceneObject)
        {
            if (sceneObject == null) { throw new ArgumentNullException("sceneObject"); }
            if (sceneObject.Scene == m_scene) { return; }
            if (sceneObject.Scene != null) { throw new ArgumentException("Given object does already belong to another scene!", "sceneObject"); }
            if (sceneObject.SceneLayer == this) { return; }
            if (sceneObject.SceneLayer != null) { throw new ArgumentException("Given object does already belong to another scene layer!", "sceneObject"); }

            m_sceneObjects.Add(sceneObject);
            sceneObject.Scene = m_scene;
            sceneObject.SceneLayer = this;
        }

        /// <summary>
        /// Clears this layer.
        /// </summary>
        public void Clear()
        {
            foreach (SceneObject actObject in m_sceneObjects)
            {
                if (actObject.IsLoaded) { actObject.UnloadResources(); }
                actObject.UnsubsribeFromAllPasses();
                actObject.Scene = null;
            }
            m_sceneObjects.Clear();
        }

        /// <summary>
        /// Removes the given object from the layer.
        /// </summary>
        /// <param name="sceneObject">Object to remove.</param>
        public void Remove(SceneObject sceneObject)
        {
            if (m_sceneObjects.Contains(sceneObject))
            {
                if (sceneObject.IsLoaded) { sceneObject.UnloadResources(); }

                sceneObject.UnsubsribeFromAllPasses();
                m_sceneObjects.Remove(sceneObject);
                sceneObject.Scene = null;
            }
        }

        /// <summary>
        /// Unloads all resources.
        /// </summary>
        internal void UnloadResources()
        {
            //Unload resources of all scene objects
            foreach (SceneObject actObject in m_sceneObjects)
            {
                if (actObject.IsLoaded)
                {
                    actObject.UnloadResources();
                }
            }
        }

        /// <summary>
        /// Prepares rendering (Loads all needed resources).
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        internal void PrepareRendering(D3D11.Device device)
        {
            List<SceneObject> invalidObjects = null;

            //Load all resources
            foreach (SceneObject actObject in m_sceneObjects)
            {
                try
                {
                    //Load all resources of the object
                    if (!actObject.IsLoaded)
                    {
                        actObject.LoadResources(device);
                    }
                }
                catch (Exception ex)
                {
                    //Build list of invalid objects
                    if (invalidObjects == null) { invalidObjects = new List<SceneObject>(); }
                    invalidObjects.Add(actObject);
                }
            }

            //Remove all invalid objects
            if (invalidObjects != null)
            {
                HandleInvalidObjects(invalidObjects);
            }
        }

        /// <summary>
        /// Updates the layer.
        /// </summary>
        /// <param name="updateState">Current update state.</param>
        internal void Update(UpdateState updateState)
        {
            updateState.SceneLayer = this;
            try
            {
                SceneObject[] updateList = m_sceneObjects.ToArray();
                for (int loop = 0; loop < updateList.Length; loop++)
                {
                    updateList[loop].Update(updateState);
                }
            }
            finally
            {
                updateState.SceneLayer = null;
            }
        }

        /// <summary>
        /// Renders the scene to the given context.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        internal void Render(RenderState renderState)
        {
            List<SceneObject> invalidObjects = null;

            //RaiseBeginRender(renderState);

            //Perform all plain renderings
            if (m_objectsPassPlainRender.Count > 0)
            {
                for (int loopPass = 0; loopPass < m_objectsPassPlainRender.Count; loopPass++)
                {
                    RenderPassSubscription actSubscription = m_objectsPassPlainRender[loopPass];
                    try
                    {
                        actSubscription.RenderMethod(renderState);
                    }
                    catch (Exception ex)
                    {
                        //TODO: What to do with Exception object???

                        if (invalidObjects == null) { invalidObjects = new List<SceneObject>(); }
                        invalidObjects.Add(actSubscription.SceneObject);
                    }
                }
            }

            //Perform all transparent renderings
            if (m_objectsPassTransparentRender.Count > 0)
            {
                //Ensure loaded resources for transparency pass
                if (!m_renderPassTransparent.IsLoaded)
                {
                    m_renderPassTransparent.LoadResource(this.Scene.Resources);
                }

                //Render all transparent objects
                m_renderPassTransparent.Apply(renderState);
                try
                {
                    for (int loopPass = 0; loopPass < m_objectsPassTransparentRender.Count; loopPass++)
                    {
                        RenderPassSubscription actSubscription = m_objectsPassTransparentRender[loopPass];
                        try
                        {
                            actSubscription.RenderMethod(renderState);
                        }
                        catch (Exception ex)
                        {
                            //TODO: What to do with Exception object???

                            if (invalidObjects == null) { invalidObjects = new List<SceneObject>(); }
                            invalidObjects.Add(actSubscription.SceneObject);
                        }
                    }
                }
                finally
                {
                    m_renderPassTransparent.Discard(renderState);
                }
            }

            ////Render all objects
            //foreach (SceneObject actObject in m_sceneObjects)
            //{
            //    //Load all resources of the object
            //    try
            //    {
            //        //Render the object
            //        if (actObject.IsLoaded)
            //        {
            //            actObject.Render(renderState);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        //Build list of invalid objects
            //        if (invalidObjects == null) { invalidObjects = new List<SceneObject>(); }
            //        invalidObjects.Add(actObject);
            //    }
            //}

            //Remove all invalid objects
            if (invalidObjects != null)
            {
                HandleInvalidObjects(invalidObjects);
            }

            //RaiseFinishRender(renderState);
        }

        /// <summary>
        /// Handles invalid objects.
        /// </summary>
        /// <param name="invalidObjects">List containing all invalid objects to handle.</param>
        private void HandleInvalidObjects(List<SceneObject> invalidObjects)
        {
            foreach (SceneObject actObject in invalidObjects)
            {
                //Unload the object if it is loaded
                if (actObject.IsLoaded)
                {
                    try { actObject.UnloadResources(); }
                    catch (Exception) { }
                }

                actObject.UnsubsribeFromAllPasses();
                m_sceneObjects.Remove(actObject);
            }
        }

        ///// <summary>
        ///// Raises the BeginRender event.
        ///// </summary>
        //private void RaiseBeginRender(RenderState renderState)
        //{
        //    if (BeginRender != null) { BeginRender(this, new Rendering3DArgs(renderState)); }
        //}

        ///// <summary>
        ///// Raises the FinishRender event.
        ///// </summary>
        //private void RaiseFinishRender(RenderState renderState)
        //{
        //    if (FinishRender != null) { FinishRender(this, new Rendering3DArgs(renderState)); }
        //}

        /// <summary>
        /// Gets the name of this layer.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets parent scene.
        /// </summary>
        public Scene Scene
        {
            get { return m_scene; }
        }

        /// <summary>
        /// Gets a collection containing all objects.
        /// </summary>
        public ReadOnlyCollection<SceneObject> Objects
        {
            get { return m_sceneObjectsPublic; }
        }

        /// <summary>
        /// Gets total count of objects within the scene.
        /// </summary>
        public int CountObjects
        {
            get { return m_sceneObjects.Count; }
        }

    }
}
