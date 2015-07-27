using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using RK.Common;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.GraphicsEngine.Objects;

namespace RK.Wpf3DSampleBrowser.Samples.DXInstancing
{
    /// <summary>
    /// Interaction logic for DXInstancingControl.xaml
    /// </summary>
    [Export(Infrastructure.SAMPLE_CONTRACT, typeof(UserControl))]
    [Sample(SampleType.SharpDXSample, 6, "pack://application:,,,/RK.Wpf3DSampleBrowser;component/Resources/Icons/Text3D32x32.png")]
    [DisplayName("Instancing")]
    public partial class DXInstancingControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DXInstancingControl" /> class.
        /// </summary>
        public DXInstancingControl()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;

            //this.InvokeDelayedWhileWpf(
            //    () => true,
            //    () => m_lstPerformanceValues.Items.Refresh(),
            //    TimeSpan.FromMilliseconds(500.0));
        }

        /// <summary>
        /// Initializes the 3d scene.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Scene scene = this.FindResource("Scene") as Scene;

            //Generate text resource
            TextGeometryOptions geoOptions = TextGeometryOptions.Default;
            geoOptions.SurfaceVertexColor = Color4.DarkBlue;
            geoOptions.VolumetricSideSurfaceVertexColor = Color4.LightSteelBlue;
            geoOptions.VolumetricTextDepth = 3f;
            scene.Resources.AddTextGeometry("TextGeometry", "Game board", geoOptions);

            //Generate all objects
            DemoInstancedGenericObject instancedObjectHost = new DemoInstancedGenericObject("TextGeometry");
            for (int loopX = 0; loopX < 10; loopX++)
            {
                for (int loopY = 0; loopY < 10; loopY++)
                {
                    for (int loopZ = 0; loopZ < 10; loopZ++)
                    {
                        Vector3 instancePosition = new Vector3(loopX * 8f, loopY * 3f, loopZ * 1.5f);

                        //Generate instancing data
                        instancedObjectHost.InstanceTransformations.Add(Matrix4.Translation(instancePosition));

                        //Generate standard object
                        //scene.Add(new GenericObject("TextGeometry", instancePosition));
                    }
                }
            }

            //instancedObjectHost.Rendered += (innerSender, innerArgs) => Thread.Sleep(50);
            scene.Add(instancedObjectHost);

            //Configure the camera
            //Camera camera = m_direct3DImage.Camera;
            Camera camera = this.FindResource("Camera") as Camera;
            camera.Position = new Vector3(0f, 1.5f, 0f);
            camera.RelativeTarget = new Common.Vector3(0f, 0f, 1f);
            camera.TargetRotation = new Vector2(-(float)Math.PI / 4f, -(float)Math.PI / 7f);
            camera.Zoom(-10f);
            camera.UpdateCamera();
        }

        private void OnCmdToggleWireframeClick(object sender, RoutedEventArgs e)
        {
            //m_direct3DImage.IsWireframeEnabled = !m_direct3DImage.IsWireframeEnabled;
        }
    }
}