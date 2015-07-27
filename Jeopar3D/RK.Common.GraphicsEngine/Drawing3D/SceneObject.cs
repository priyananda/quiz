using System;
using System.Collections.Generic;
using RK.Common.GraphicsEngine.Core;
using RK.Common.GraphicsEngine.Drawing3D.Resources;
using RK.Common.Util;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace RK.Common.GraphicsEngine.Drawing3D
{
    public abstract class SceneObject
    {
        //Some information about parent containers
        private Scene m_scene;

        private SceneLayer m_sceneLayer;

        //Members for animations
        private AnimationHandler m_animationHandler;

        //Generic data
        private List<string> m_visibilityLayers;

        private List<RenderPassSubscription> m_renderPassSubscriptions;

        //public event Rendering3DHandler Rendered;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneObject"/> class.
        /// </summary>
        protected SceneObject()
        {
            //m_animations = new List<ObjectAnimation>();
            m_visibilityLayers = new List<string>();
            m_animationHandler = new AnimationHandler(this);
            m_renderPassSubscriptions = new List<RenderPassSubscription>();
        }

        /// <summary>
        /// Starts a new AnimationSequence.
        /// </summary>
        public AnimationSequenceBuilder BeginAnimationSequence()
        {
            return new AnimationSequenceBuilder(m_animationHandler);
        }

        /// <summary>
        /// Performs a simple picking-test.
        /// </summary>
        /// <param name="pickingRay">The picking ray.</param>
        public virtual PickingInformation Pick(Ray pickingRay)
        {
            if (m_scene == null) { throw new GraphicsEngineException("SceneObject does not belong to a scene!"); }
            if (m_sceneLayer == null) { throw new GraphicsEngineException("SceneObject does not belong to a scene!"); }

            return null;
        }

        /// <summary>
        /// Loads all resources of the object.
        /// </summary>
        /// <param name="device">Current DirectX device.</param>
        public abstract void LoadResources(D3D11.Device device);

        /// <summary>
        /// Updates this object.
        /// </summary>
        /// <param name="updateState">State of update process.</param>
        public void Update(UpdateState updateState)
        {
            //Update current animation state
            if (m_animationHandler != null)
            {
                m_animationHandler.Update(updateState);
            }

            //Update the object
            UpdateInternal(updateState);
        }

        /// <summary>
        /// Unloads all resources of the object.
        /// </summary>
        public abstract void UnloadResources();

        /// <summary>
        /// Subscribes on the given render pass with the given action.
        /// </summary>
        /// <param name="renderPass">The pass to register on.</param>
        /// <param name="updateState">The update state used for registering.</param>
        /// <param name="renderAction">The target action to be invoked on render.</param>
        protected internal void SubscribeToPass(RenderPassInfo renderPass, UpdateState updateState, Action<RenderState> renderAction)
        {
            m_renderPassSubscriptions.Add(updateState.SceneLayer.SubscribeForPass(renderPass, this, renderAction));
        }

        /// <summary>
        /// Unsubscribes from the given render pass.
        /// </summary>
        /// <param name="passInfo">The pass to unsubscribe from.</param>
        protected internal void UnsubscribeFromPass(RenderPassInfo passInfo)
        {
            for (int loop = 0; loop < m_renderPassSubscriptions.Count; loop++)
            {
                if (m_renderPassSubscriptions[loop].RenderPass == passInfo)
                {
                    m_renderPassSubscriptions[loop].Unsubscribe();
                    m_renderPassSubscriptions.RemoveAt(loop);
                    return;
                }
            }
        }

        /// <summary>
        /// Unsubscribes from all passes.
        /// </summary>
        protected internal void UnsubsribeFromAllPasses()
        {
            for (int loop = 0; loop < m_renderPassSubscriptions.Count; loop++)
            {
                m_renderPassSubscriptions[loop].Unsubscribe();
            }
            m_renderPassSubscriptions.Clear();
        }

        ///// <summary>
        ///// Renders the object.
        ///// </summary>
        ///// <param name="renderState">Current render state.</param>
        //public void Render(RenderState renderState)
        //{
        //    //Stopwatch stopwatch = new Stopwatch();
        //    //stopwatch.Start();
        //    //try
        //    //{
        //        //Render the object
        //        RenderInternal(renderState);

        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="updateState">Current update state.</param>
        protected abstract void UpdateInternal(UpdateState updateState);

        //    //    //Raise Rendered event
        //    //    RaiseRendered(renderState);
        //    //}
        //    //finally
        //    //{
        //    //    stopwatch.Stop();
        //    //    m_lastRenderTime = (int)stopwatch.ElapsedMilliseconds;
        //    //}
        //}
        /// <summary>
        /// Are resources loaded?
        /// </summary>
        public abstract bool IsLoaded { get; }

        ///// <summary>
        ///// Renders the object.
        ///// </summary>
        ///// <param name="renderState">Current render state.</param>
        //protected abstract void RenderInternal(RenderState renderState);
        ///// <summary>
        ///// Raises the Rendered event.
        ///// </summary>
        ///// <param name="renderState">Current RenderState object.</param>
        //private void RaiseRendered(RenderState renderState)
        //{
        //    if (Rendered != null) { Rendered(this, new Rendering3DArgs(renderState)); }
        //}

        /// <summary>
        /// Gets current scene.
        /// </summary>
        public Scene Scene
        {
            get { return m_scene; }
            internal set { m_scene = value; }
        }

        /// <summary>
        /// Gets or sets the scene layer.
        /// </summary>
        public SceneLayer SceneLayer
        {
            get { return m_sceneLayer; }
            internal set { m_sceneLayer = value; }
        }

        /// <summary>
        /// Gets a list containing all visibility layers wich this object belongs to.
        /// </summary>
        public List<string> VisibilityLayers
        {
            get { return m_visibilityLayers; }
        }

        /// <summary>
        /// Gets a dictionary containing all resources.
        /// </summary>
        public ResourceDictionary Resources
        {
            get
            {
                if (m_scene == null) { return null; }
                return m_scene.Resources;
            }
        }

        /// <summary>
        /// Gets current AnimationHandler object.
        /// </summary>
        public AnimationHandler AnimationHandler
        {
            get { return m_animationHandler; }
        }

        /// <summary>
        /// Gets or sets some additional methadata object.
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the total count of render pass subscriptions.
        /// </summary>
        public int RenderPassSubscriptionCount
        {
            get { return m_renderPassSubscriptions.Count; }
        }
    }
}