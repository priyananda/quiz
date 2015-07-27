using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.Util;
using Windows.UI.Xaml.Controls;

namespace RK.Common.GraphicsEngine.Gui
{
    public class Viewport3DControl : SwapChainBackgroundPanel
    {
        private BackgroundPanelDirectXCanvas m_renderTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport3DControl" /> class.
        /// </summary>
        public Viewport3DControl()
        {
            m_renderTarget = new BackgroundPanelDirectXCanvas(this);
        }

        /// <summary>
        /// Gets the 3D resource dictionary.
        /// </summary>
        public RK.Common.GraphicsEngine.Drawing3D.Resources.ResourceDictionary Resources3D
        {
            get { return m_renderTarget.Scene.Resources; }
        }

        /// <summary>
        /// Gets the current performance calculator object.
        /// </summary>
        public ActivityPerformanceValueContainer PerformanceCalculator
        {
            get { return m_renderTarget.PerformanceCalculator; }
        }

        /// <summary>
        /// Gets or sets the scene of this panel.
        /// </summary>
        public Scene Scene
        {
            get { return m_renderTarget.Scene; }
            set { m_renderTarget.Scene = value; }
        }

        /// <summary>
        /// Gets or sets the camera of this 3d panel.
        /// </summary>
        public Camera Camera
        {
            get { return m_renderTarget.Camera; }
            set { m_renderTarget.Camera = value; }
        }
    }
}