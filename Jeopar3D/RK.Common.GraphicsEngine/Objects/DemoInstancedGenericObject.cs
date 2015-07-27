using System.Collections.Generic;
using RK.Common.GraphicsEngine.Core;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.GraphicsEngine.Drawing3D.Resources;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace RK.Common.GraphicsEngine.Objects
{
    public class DemoInstancedGenericObject : SceneSpacialObject
    {
        //Configuration members
        private string m_geometry;

        private List<Matrix4> m_instanceTransormations;

        //Resources
        private GeometryResource m_geometryResource;

        private StandardPerInstanceData[] m_cachedInstanceData;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericObject"/> class.
        /// </summary>
        /// <param name="geometryResource">The geometry resource.</param>
        public DemoInstancedGenericObject(string geometryResource)
        {
            m_instanceTransormations = new List<Matrix4>();
            m_geometry = geometryResource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericObject" /> class.
        /// </summary>
        /// <param name="geometryResource">The geometry resource.</param>
        /// <param name="position">The initial position.</param>
        public DemoInstancedGenericObject(string geometryResource, Vector3 position)
        {
            m_instanceTransormations = new List<Matrix4>();
            m_geometry = geometryResource;
            this.Position = position;
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
        /// Updates the object.
        /// </summary>
        /// <param name="updateState">Current update state.</param>
        protected override void UpdateInternal(UpdateState updateState)
        {
            base.UpdateInternal(updateState);

            //Subscribe to render passes
            if (base.RenderPassSubscriptionCount == 0)
            {
                base.SubscribeToPass(
                    RenderPassInfo.PASS_PLAIN_RENDER,
                    updateState, OnRenderPlain);
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
                if (m_instanceTransormations.Count == 0) { return; }

                //Generate instance data array
                if (m_cachedInstanceData == null)
                {
                    m_cachedInstanceData = new StandardPerInstanceData[m_instanceTransormations.Count];
                    for (int loopInstance = 0; loopInstance < m_instanceTransormations.Count; loopInstance++)
                    {
                        m_cachedInstanceData[loopInstance].InstanceTransform = m_instanceTransormations[loopInstance];
                    }
                }

                //Render geometry
                m_geometryResource.RenderInstanced(renderState, m_cachedInstanceData);
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
        /// Gets current GeometryResource object.
        /// </summary>
        public GeometryResource GeometryResource
        {
            get { return m_geometryResource; }
        }

        /// <summary>
        /// Gets a list containing all instance transformation matrices.
        /// </summary>
        public List<Matrix4> InstanceTransformations
        {
            get { return m_instanceTransormations; }
        }
    }
}