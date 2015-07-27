using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using RK.Common;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.GraphicsEngine.Objects;

namespace RK.Wpf3DSampleBrowser.Samples.DXText3D
{
    /// <summary>
    /// Interaction logic for DXText3DControl.xaml
    /// </summary>
    [Export(Infrastructure.SAMPLE_CONTRACT, typeof(UserControl))]
    [Sample(SampleType.SharpDXSample, 5, "pack://application:,,,/RK.Wpf3DSampleBrowser;component/Resources/Icons/Text3D32x32.png")]
    [DisplayName("Text 3D")]
    public partial class DXText3DControl : UserControl
    {
        public DXText3DControl()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// Initializes the 3d scene.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TextGeometryOptions geoOptions = TextGeometryOptions.Default;
            geoOptions.GenerateCubesOnVertices = true;
            geoOptions.SurfaceVertexColor = Color4.DarkBlue;
            geoOptions.VolumetricSideSurfaceVertexColor = Color4.LightSteelBlue;
            geoOptions.VolumetricTextDepth = 3f;

            m_direct3DImage.Resources3D.AddTextGeometry("TextGeometry", "Game board", geoOptions);
            m_direct3DImage.Scene.Add(new GenericObject("TextGeometry"));

            m_direct3DImage.Resources3D.AddTextGeometry("TextGeometry2", "abcdefghijklmnopqrstuvwxyz", geoOptions);
            m_direct3DImage.Scene.Add(new GenericObject("TextGeometry2") { Position = new Vector3(0f, 0f, 1f) });

            m_direct3DImage.Resources3D.AddTextGeometry("TextGeometry3", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", geoOptions);
            m_direct3DImage.Scene.Add(new GenericObject("TextGeometry3") { Position = new Vector3(0f, 0f, 2f) });

            m_direct3DImage.Resources3D.AddTextGeometry("TextGeometry4", "0123456789", geoOptions);
            m_direct3DImage.Scene.Add(new GenericObject("TextGeometry4") { Position = new Vector3(0f, 0f, 3f) });

            //Configure the camera
            Camera camera = m_direct3DImage.Camera;
            camera.Position = new Vector3(0f, 1.5f, 0f);
            camera.RelativeTarget = new Common.Vector3(0f, 0f, 1f);
            camera.TargetRotation = new Vector2(-(float)Math.PI / 4f, -(float)Math.PI / 7f);
            camera.Zoom(-10f);
            camera.UpdateCamera();
        }

        private void OnCmdToggleWireframeClick(object sender, RoutedEventArgs e)
        {
            m_direct3DImage.IsWireframeEnabled = !m_direct3DImage.IsWireframeEnabled;
        }
    }
}