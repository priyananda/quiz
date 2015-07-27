using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using RK.Common;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.GraphicsEngine.Objects;

namespace RK.Wpf3DSampleBrowser.Samples.DXTransformations
{
    /// <summary>
    /// Interaction logic for DXTransformationsControl.xaml
    /// </summary>
    [Export(Infrastructure.SAMPLE_CONTRACT, typeof(UserControl))]
    [Sample(SampleType.SharpDXSample, 8, "pack://application:,,,/RK.Wpf3DSampleBrowser;component/Resources/Icons/Text3D32x32.png")]
    [DisplayName("Transformations")]
    public partial class DXTransformationsControl : UserControl
    {
        public static readonly DependencyProperty ObjectHorizontalRotationProperty =
            DependencyProperty.Register("ObjectHorizontalRotation", typeof(float), typeof(DXTransformationsControl), new PropertyMetadata(0f));

        public static readonly DependencyProperty ObjectVerticalRotationProperty =
            DependencyProperty.Register("ObjectVerticalRotation", typeof(float), typeof(DXTransformationsControl), new PropertyMetadata(0f));

        public static readonly DependencyProperty ObjectRotationTypeProperty =
            DependencyProperty.Register("ObjectRotationType", typeof(SpacialTransformationType), typeof(DXTransformationsControl), new PropertyMetadata(SpacialTransformationType.ScalingTranslationHVAngles));

        public static readonly DependencyProperty ObjectPitchRotationProperty =
            DependencyProperty.Register("ObjectPitchRotation", typeof(float), typeof(DXTransformationsControl), new PropertyMetadata(0f));

        public static readonly DependencyProperty ObjectYawRotationProperty =
            DependencyProperty.Register("ObjectYawRotation", typeof(float), typeof(DXTransformationsControl), new PropertyMetadata(0f));

        public static readonly DependencyProperty ObjectRollRotationProperty =
            DependencyProperty.Register("ObjectRollRotation", typeof(float), typeof(DXTransformationsControl), new PropertyMetadata(0f));

        private List<SceneSpacialObject> m_transformedObjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="DXTransformationsControl" /> class.
        /// </summary>
        public DXTransformationsControl()
        {
            m_transformedObjects = new List<SceneSpacialObject>();

            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// Default load event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CreateFloor(
                new Vector2(2f, 2f),
                10, 10);

            m_direct3DImage.Scene.BeforeRendering += OnScneBeforeRendering;

            VertexStructure boneStructure = new VertexStructure();
            boneStructure.BuildCube24V(Vector3.Empty, 1f, 1f, Color4.Black);
            boneStructure.Material = "BoneMaterial";
            boneStructure.TransformVertices(Common.Matrix4.RotationYawPitchRoll(0f, 0f, EngineMath.RAD_270DEG));
            boneStructure.TransformVertices(Common.Matrix4.RotationYawPitchRoll(EngineMath.RAD_180DEG, 0f, 0f));
            m_direct3DImage.Resources3D.AddGeometry("BoneGeometry", boneStructure.ToSingleItemArray());

            GenericObject line3DObject = new GenericObject("BoneGeometry");
            line3DObject.Position = new Vector3(3f, 0f, 0f);
            line3DObject.TransformationType = SpacialTransformationType.ScalingTranslationHVAngles;
            line3DObject.RotationHV = new Vector2();
            line3DObject.Scaling = new Vector3(1f, 0.03f, 0.03f);
            m_direct3DImage.Scene.Add(line3DObject);
            m_transformedObjects.Add(line3DObject);

            m_direct3DImage.Resources3D.AddGeometry("ForkliftGeometry", ObjectType.FromACFile(new Uri("pack://application:,,,/RK.Wpf3DSampleBrowser;component/Resources/Models/Forklift.ac")));
            GenericObject forkliftObject = m_direct3DImage.Scene.Add(new GenericObject("ForkliftGeometry")) as GenericObject;
            forkliftObject.Position = new Vector3(3f, 0f, -5f);
            m_transformedObjects.Add(forkliftObject);

            m_direct3DImage.Resources3D.AddGeometry("BananaGeometry", ObjectType.FromACFile(new Uri("pack://application:,,,/RK.Wpf3DSampleBrowser;component/Resources/Models/Banana.ac")));
            GenericObject bananaObject = m_direct3DImage.Scene.Add(new GenericObject("BananaGeometry")) as GenericObject;
            bananaObject.Position = new Vector3(3f, 0f, -2.5f);
            m_transformedObjects.Add(bananaObject);

            VertexStructure vStructure = new VertexStructure();
            vStructure.BuildCylinderFullV(Vector3.Empty, 0.1f, 1.6f, 20, Color4.Red);
            vStructure.BuildConeFullV(
                new Vector3(0f, 1.6f, 0f),
                0.2f, 0.4f, 20, Color4.Red);
            vStructure.Material = "ArrowMaterial";
            m_direct3DImage.Resources3D.AddGeometry("ArrowGeometry", vStructure.ToSingleItemArray());

            m_direct3DImage.Scene.Add(new GenericObject("ArrowGeometry"));
            m_direct3DImage.Scene.Add(new GenericObject("ArrowGeometry") { Rotation = new Vector3(EngineMath.RAD_90DEG, 0f, 0f) });
            m_direct3DImage.Scene.Add(new GenericObject("ArrowGeometry") { Rotation = new Vector3(0f, 0f, -EngineMath.RAD_90DEG) });

            //Configure the camera
            Camera camera = m_direct3DImage.Camera;
            camera.Position = new Vector3(0f, 1.5f, 0f);
            camera.RelativeTarget = new Common.Vector3(0f, 0f, 1f);
            camera.TargetRotation = new Vector2(-(float)Math.PI / 4f, -(float)Math.PI / 7f);
            camera.Zoom(-10f);
            camera.UpdateCamera();
        }

        /// <summary>
        /// Update values on current dummy object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnScneBeforeRendering(object sender, Rendering3DArgs e)
        {
            foreach (var actTransformedObject in m_transformedObjects)
            {
                actTransformedObject.TransformationType = this.ObjectRotationType;
                actTransformedObject.RotationHV = new Vector2(this.ObjectHorizontalRotation, this.ObjectVerticalRotation);
                actTransformedObject.Rotation = new Vector3(
                    this.ObjectPitchRotation,
                    this.ObjectYawRotation,
                    this.ObjectRollRotation);
            }
        }

        /// <summary>
        /// Creates the floor
        /// </summary>
        /// <param name="tileSize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void CreateFloor(Vector2 tileSize, int width, int height)
        {
            m_direct3DImage.Resources3D.AddTexture("FloorTexture", new Uri("pack://application:,,,/RK.Wpf3DSampleBrowser;component/Resources/Textures/Floor.png"));
            m_direct3DImage.Resources3D.AddSimpleTexturedMaterial("FloorMaterial", "FloorTexture");
            m_direct3DImage.Resources3D.AddSimpleColoredMaterial("FloorSideMaterial");

            FloorType FloorType = new FloorType(tileSize, 0f);
            FloorType.BorderMaterial = "FloorMaterial";
            FloorType.BottomMaterial = "FloorMaterial";
            FloorType.DefaultFloorMaterial = "FloorMaterial";
            FloorType.SideMaterial = "FloorSideMaterial";
            FloorType.SetTilemap(width, height);

            m_direct3DImage.Resources3D.AddGeometry("FloorGeometry", FloorType);
            m_direct3DImage.Scene.Add(new GenericObject("FloorGeometry"));
        }

        public float ObjectHorizontalRotation
        {
            get { return (float)GetValue(ObjectHorizontalRotationProperty); }
            set { SetValue(ObjectHorizontalRotationProperty, value); }
        }

        public float ObjectVerticalRotation
        {
            get { return (float)GetValue(ObjectVerticalRotationProperty); }
            set { SetValue(ObjectVerticalRotationProperty, value); }
        }

        public SpacialTransformationType ObjectRotationType
        {
            get { return (SpacialTransformationType)GetValue(ObjectRotationTypeProperty); }
            set { SetValue(ObjectRotationTypeProperty, value); }
        }

        public float ObjectYawRotation
        {
            get { return (float)GetValue(ObjectYawRotationProperty); }
            set { SetValue(ObjectYawRotationProperty, value); }
        }

        public float ObjectPitchRotation
        {
            get { return (float)GetValue(ObjectPitchRotationProperty); }
            set { SetValue(ObjectPitchRotationProperty, value); }
        }

        public float ObjectRollRotation
        {
            get { return (float)GetValue(ObjectRollRotationProperty); }
            set { SetValue(ObjectRollRotationProperty, value); }
        }
    }
}