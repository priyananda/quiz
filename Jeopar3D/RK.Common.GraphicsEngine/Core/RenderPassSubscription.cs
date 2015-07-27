using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RK.Common.GraphicsEngine.Drawing3D;

namespace RK.Common.GraphicsEngine.Core
{
    public struct RenderPassSubscription
    {
        internal SceneLayer SceneLayer;
        internal RenderPassInfo RenderPass;
        internal SceneObject SceneObject;
        internal Action<RenderState> RenderMethod;

        internal RenderPassSubscription(SceneLayer sceneLayer, RenderPassInfo renderPass, SceneObject sceneObject, Action<RenderState> renderMethod)
        {
            SceneLayer = sceneLayer;
            RenderPass = renderPass;
            SceneObject = sceneObject;
            RenderMethod = renderMethod;
        }

        /// <summary>
        /// Unsubscribes this subscription.
        /// </summary>
        public void Unsubscribe()
        {
            if (SceneObject != null)
            {
                SceneLayer.UnsubscribeForPass(RenderPass, SceneObject);
                SceneLayer = null;
                RenderPass = null;
                SceneObject = null;
                RenderMethod = null;
            }
        }

        public bool Subscribed
        {
            get { return SceneObject != null; }
        }
    }
}
