using System;
using RK.Common.GraphicsEngine.Core;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.GraphicsEngine.Drawing3D.Resources;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace RK.Common.GraphicsEngine.Objects
{
    public class GenericObject : SceneSpacialObject
    {
        //Configuration members
        private string m_geometry;

        private float m_opacity;
        private bool m_passRelevantValuesChanged;

        //Resources
        private GeometryResource m_geometryResource;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericObject"/> class.
        /// </summary>
        /// <param name="geometryResource">The geometry resource.</param>
        public GenericObject(string geometryResource)
        {
            m_geometry = geometryResource;
            m_opacity = 1f;
            m_passRelevantValuesChanged = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericObject" /> class.
        /// </summary>
        /// <param name="geometryResource">The geometry resource.</param>
        /// <param name="position">The initial position.</param>
        public GenericObject(string geometryResource, Vector3 position)
            : this(geometryResource)
        {
            this.Position = position;
        }

        public override string ToString()
        {
            return "GenericObject (Geometry: " + m_geometry + ")";
        }

        /// <summary>
        /// Performs a simple picking-test.
        /// </summary>
        /// <param name="pickingRay">The picking ray.</param>
        /// <returns></returns>
        public override PickingInformation Pick(Ray pickingRay)
        {
            ObjectType objType = GetObjectType();

            //Transfrom given ray to object space
            Matrix4 localTransform = base.Transform;
            localTransform.Invert();
            pickingRay.Transform(localTransform);

            return null;
        }

        /// <summary>
        /// Tries to get the ObjectType.
        /// </summary>
        public ObjectType GetObjectType()
        {
            if (m_geometryResource != null) { return m_geometryResource.ObjectType; }
            else
            {
                if ((base.Resources != null) && (base.Resources.ContainsResource(m_geometry)))
                {
                    GeometryResource geometryResource = base.Resources[m_geometry] as GeometryResource;
                    if (geometryResource != null)
                    {
                        return geometryResource.ObjectType;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Changes the geometry to the given one.
        /// </summary>
        /// <param name="newGeometry">The new geometry to set.</param>
        public void ChangeGeometry(string newGeometry)
        {
            this.UnloadResources();

            m_geometry = newGeometry;
        }

        /// <summary>
        /// Loads all resources of the object.
        /// </summary>
        /// <param name="device">Current DirectX device.</param>
        public override void LoadResources(D3D11.Device device)
        {
            //Load geometry resource
            m_geometryResource = base.Resources.GetResourceAndEnsureLoaded<GeometryResource>(m_geometry);
        }

        /// <summary>
        /// Unloads all resources of the object.
        /// </summary>
        public override void UnloadResources()
        {
            m_geometryResource = null;
        }

        /// <summary>
        /// Standard update logic of this object.
        /// </summary>
        /// <param name="updateState">Current update state.</param>
        protected override void UpdateInternal(UpdateState updateState)
        {
            base.UpdateInternal(updateState);

            //Subscribe to render passes
            if ((m_passRelevantValuesChanged) ||
                (base.RenderPassSubscriptionCount == 0))
            {
                //Unsubscribe from all passes first
                base.UnsubsribeFromAllPasses();

                //Now subscribe to needed pass
                if (m_opacity < 1f)
                {
                    base.SubscribeToPass(
                        RenderPassInfo.PASS_TRANSPARENT_RENDER,
                        updateState, OnRenderTransparent);
                }
                else
                {
                    base.SubscribeToPass(
                        RenderPassInfo.PASS_PLAIN_RENDER,
                        updateState, OnRenderPlain);
                }

                //Update local flag
                m_passRelevantValuesChanged = false;
            }
        }

        /// <summary>
        /// Renders the object.
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        private void OnRenderPlain(RenderState renderState)
        {
            Matrix4Stack matrixStack = renderState.World;
            matrixStack.Push(base.Transform);
            try
            {
                //Render geometry
                m_geometryResource.Render(renderState);
            }
            finally
            {
                matrixStack.Pop();
            }
        }

        /// <summary>
        /// Renders the object.
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        private void OnRenderTransparent(RenderState renderState)
        {
            //Apply opacity value
            renderState.ObjectOpacity = Math.Max(0f, m_opacity);

            //Render the geometry
            Matrix4Stack matrixStack = renderState.World;
            matrixStack.Push(base.Transform);
            try
            {
                m_geometryResource.Render(renderState);
            }
            finally
            {
                matrixStack.Pop();
            }
        }

        /// <summary>
        /// Are resources loaded?
        /// </summary>
        /// <value></value>
        public override bool IsLoaded
        {
            get { return m_geometryResource != null; }
        }

        /// <summary>
        /// Gets or sets current opacity value.
        /// </summary>
        public float Opacity
        {
            get { return m_opacity; }
            set
            {
                if (m_opacity != value)
                {
                    m_opacity = value;
                    m_passRelevantValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets current GeometryResource object.
        /// </summary>
        public GeometryResource GeometryResource
        {
            get { return m_geometryResource; }
        }
    }
}