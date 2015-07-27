using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RK.Common.GraphicsEngine.Drawing3D;

namespace RK.Wpf3DSampleBrowser.Util
{
    /// <summary>
    /// Interaction logic for WpfDirectXSampleHost.xaml
    /// </summary>
    public partial class WpfDirectXSampleHost : UserControl
    {
        public static readonly DependencyProperty SceneProperty =
            DependencyProperty.Register("Scene", typeof(Scene), typeof(WpfDirectXSampleHost), new PropertyMetadata(null));
        public static readonly DependencyProperty CameraProperty =
            DependencyProperty.Register("Camera", typeof(Camera), typeof(WpfDirectXSampleHost), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfDirectXSampleHost" /> class.
        /// </summary>
        public WpfDirectXSampleHost()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// Updates the view values.
        /// </summary>
        private void UpdateViewValues()
        {
            if (m_view3DHost == null) { return; }

            //Update wpf host
            WpfD3DImageHost wpfHost = m_view3DHost.Content as WpfD3DImageHost;
            if (wpfHost != null)
            {
                wpfHost.Camera = this.Camera;
                wpfHost.Scene = this.Scene;
                return;
            }

            //Update windows.forms host
            WpfWinFormsHost winFormsHost = m_view3DHost.Content as WpfWinFormsHost;
            if (winFormsHost != null)
            {
                winFormsHost.Scene = this.Scene;
                winFormsHost.Camera = this.Camera;
            }
        }

        private void ToggleTarget()
        {
            if (m_view3DHost.Content == null)
            {
                WpfWinFormsHost winFormsHost = new WpfWinFormsHost();
                m_view3DHost.Content = winFormsHost;

                //WpfD3DImageHost wpfHost = new WpfD3DImageHost();
                //m_view3DHost.Content = wpfHost;
            }
            else if (m_view3DHost.Content.GetType() == typeof(WpfD3DImageHost))
            {
                m_view3DHost.Content = null;

                WpfWinFormsHost winFormsHost = new WpfWinFormsHost();
                m_view3DHost.Content = winFormsHost;
            }
            else
            {
                m_view3DHost.Content = null;

                WpfD3DImageHost wpfHost = new WpfD3DImageHost();
                m_view3DHost.Content = wpfHost;
            }

            UpdateViewValues();
        }

        /// <summary>
        /// Called when this control is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ToggleTarget();
        }

        /// <summary>
        /// Called when any dependency property has changed.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            UpdateViewValues();
        }

        /// <summary>
        /// Toggles wireframe mode.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OnCmdToggleWireframeClick(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Called when user wants to toggle the target technologie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCmdToggleTargetClick(object sender, RoutedEventArgs e)
        {
            ToggleTarget();
        }

        /// <summary>
        /// Gets or sets current 3D scene.
        /// </summary>
        public Scene Scene
        {
            get { return (Scene)GetValue(SceneProperty); }
            set { SetValue(SceneProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current camera.
        /// </summary>
        public Camera Camera
        {
            get { return (Camera)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }
    }
}
