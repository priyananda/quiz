using System;
using RK.Common.Util;

namespace RK.Common.GraphicsEngine.Drawing3D
{
    public class UpdateState
    {
        private TimeSpan m_updateTime;
        private Matrix4Stack m_world;
        private SceneLayer m_sceneLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateState"/> class.
        /// </summary>
        /// <param name="updateTime">The update time.</param>
        public UpdateState(TimeSpan updateTime)
        {
            m_updateTime = updateTime;
            m_world = new Matrix4Stack(Matrix4.Identity);
        }

        /// <summary>
        /// Gets current update time.
        /// </summary>
        public TimeSpan UpdateTime
        {
            get { return m_updateTime; }
        }

        /// <summary>
        /// Gets current world transform.
        /// </summary>
        public Matrix4Stack World
        {
            get { return m_world; }
        }

        /// <summary>
        /// The scene layer the currently updated object belongs to.
        /// </summary>
        public SceneLayer SceneLayer
        {
            get { return m_sceneLayer; }
            internal set { m_sceneLayer = value; }
        }

        /// <summary>
        /// The scene the currently updated object belongs to.
        /// </summary>
        public Scene Scene
        {
            get
            {
                if (m_sceneLayer == null) { return null; }
                else { return m_sceneLayer.Scene; }
            }
        }
    }
}
