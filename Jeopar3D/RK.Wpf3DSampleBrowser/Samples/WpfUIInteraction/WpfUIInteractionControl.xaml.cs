using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RK.Wpf3DSampleBrowser.Samples.WpfSimpleCubes;
using Shenoy.Question.UI;
using RK.Wpf3DSampleBrowser.UI;
using Shenoy.Question.Model;

namespace RK.Wpf3DSampleBrowser.Samples.WpfUIInteraction
{
    /// <summary>
    /// Interaction logic for WpfUIInteractionControl.xaml
    /// </summary>
    [Export(Infrastructure.SAMPLE_CONTRACT, typeof(UserControl))]
    [Sample(SampleType.WpfSample, 4, "pack://application:,,,/RK.Wpf3DSampleBrowser;component/Resources/Icons/UIInteraction32x32.png")]
    [DisplayName("QuizPane")]
    public partial class WpfUIInteractionControl : UserControl
    {
        private QuestionGrid m_questionGrid = new QuestionGrid();

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfUIInteractionControl" /> class.
        /// </summary>
        public WpfUIInteractionControl()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// Loads all models.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //Build material
            Material material = null;
            material = new DiffuseMaterial(new SolidColorBrush(Colors.White));
            material.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);
            
            //Build all cubes
            const int xStart = -2, xZoom = +4;
            const int yStart = +1, yZoom = +4;
            const int zStart = -1, zZoom = -6;
            for (int i = 0; i < m_questionGrid.NumTopics; ++i)
            {
                AddTopicHeader(
                    new Point3D((xStart + i) * xZoom,(yStart + m_questionGrid.NumTypes) * yZoom, zStart * zZoom),
                    material,
                    "historylogo"
                );
                for (int j = 0; j < m_questionGrid.NumTypes; ++j)
                {
                    if (i == 0)
                    {
                        AddTypeHeader(
                            new Point3D((xStart - 1) * xZoom, (yStart + j) * yZoom, zStart * zZoom),
                            material,
                            m_questionGrid.TypeText(j),
                            m_questionGrid.QColor(new QuestionKey(0, j, 0))
                        );
                    }

                    for (int k = 0; k < m_questionGrid.NumPoints; ++k)
                    {
                        QuestionKey key = new QuestionKey(i, j, k);
                        int qid = m_questionGrid.GetQuestionId(key);
                        if (qid < 0)
                            continue;
                        var btn = new Button() { Content = m_questionGrid.QText(key) };
                        btn.Background = m_questionGrid.QColor(key);
                        btn.Click += btn_Click;
                        btn.FontFamily = new FontFamily("Calibri");
                        btn.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                        AddCube(
                            new Point3D((xStart + i) * xZoom, (yStart + j) * yZoom, (zStart + k) * zZoom),
                            material,
                            btn
                        );
                        btn.Tag = qid;
                        Questions.Get(qid).Answered += OnQuestionAnswered;
                    }
                }
            }

            m_viewport.SetCameraPos(0);

            Keyboard.Focus(this);
        }

        private void OnQuestionAnswered(Question obj)
        {
            DeleteVisuals(obj.Id);
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            try
            {
                QuestionWindow window = new QuestionWindow((int)btn.Tag);
                //window.AttachExtension(new QuestionBuzzerDashboardExt());
                window.ShowModal();
            }
            catch
            {
            }
        }

        private void DeleteVisuals(int q)
        {
            List<Visual3D> nodesToDelete = new List<Visual3D>();
            foreach (Viewport2DVisual3D obj in m_viewport.Visual3DChildren)
            {
                Button btn = obj.Visual as Button;
                if ((int)btn.Tag == q)
                    nodesToDelete.Add(obj);
            }
            foreach(var obj in nodesToDelete)
                m_viewport.Visual3DChildren.Remove(obj);
        }

        /// <summary>
        /// Adds a cube with the given material to the scene.
        /// </summary>
        /// <param name="position">Position of the cube.</param>
        /// <param name="material">Material for the cube.</param>
        private Visual3D AddCube(Point3D position, Material material, Visual visual)
        {
            //Create the mesh geometry
            MeshGeometry3D triangleMesh = new MeshGeometry3D();

            double dxHalfSize = 1;

            Point3D a = new Point3D(position.X - dxHalfSize, position.Y - dxHalfSize, position.Z + dxHalfSize);
            Point3D b = new Point3D(position.X + dxHalfSize, position.Y - dxHalfSize, position.Z + dxHalfSize);
            Point3D c = new Point3D(position.X + dxHalfSize, position.Y - dxHalfSize, position.Z - dxHalfSize);
            Point3D d = new Point3D(position.X - dxHalfSize, position.Y - dxHalfSize, position.Z - dxHalfSize);
            Point3D e = new Point3D(position.X - dxHalfSize, position.Y + dxHalfSize, position.Z + dxHalfSize);
            Point3D f = new Point3D(position.X + dxHalfSize, position.Y + dxHalfSize, position.Z + dxHalfSize);
            Point3D g = new Point3D(position.X + dxHalfSize, position.Y + dxHalfSize, position.Z - dxHalfSize);
            Point3D h = new Point3D(position.X - dxHalfSize, position.Y + dxHalfSize, position.Z - dxHalfSize);
            BuildRectangle(triangleMesh, a, b, f, e, new Vector3D(0, 0, 1));
            BuildRectangle(triangleMesh, b, c, g, f, new Vector3D(1, 0, 0));
            BuildRectangle(triangleMesh, c, d, h, g, new Vector3D(0, 0, -1));
            BuildRectangle(triangleMesh, d, a, e, h, new Vector3D(-1, 0, 0));
            BuildRectangle(triangleMesh, e, f, g, h, new Vector3D(0, 1, 0));
            BuildRectangle(triangleMesh, a, d, c, b, new Vector3D(0, -1, 0));

            Viewport2DVisual3D vp2DVisual3D = new Viewport2DVisual3D();
            vp2DVisual3D.Geometry = triangleMesh;
            vp2DVisual3D.Visual = visual;
            vp2DVisual3D.Material = material;
   
            //Add the object to the viewport
            this.m_viewport.Visual3DChildren.Add(vp2DVisual3D);

            return vp2DVisual3D;
        }

        private void AddTypeHeader(Point3D point3D, Material material, string text, Brush backBrush)
        {
            text = text + " \u21d2";
            var textBlock = new TextBlock() { Text = text };
            textBlock.Background = backBrush;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.FontWeight = FontWeights.Bold;
            AddCube(point3D, material, textBlock);
        }

        private void AddTopicHeader(Point3D point3D, Material material, string imageName)
        {
            var image = new Image();
            image.Source = MediaManager.LoadImage("data\\" + imageName + ".png", true);
            AddCube(point3D, material, image);
        }

        /// <summary>
        /// Builds a rectangle using the given points a, b, c and d and the given normal.
        /// </summary>
        /// <param name="geometry">Target geometry.</param>
        /// <param name="a">Point a of the rectangle.</param>
        /// <param name="b">Point b of the rectangle.</param>
        /// <param name="c">Point c of the rectangle.</param>
        /// <param name="d">Point d of the rectangle.</param>
        /// <param name="normal">The normal.</param>
        private void BuildRectangle(
            MeshGeometry3D geometry,
            Point3D a, Point3D b, Point3D c, Point3D d,
            Vector3D normal)
        {
            int baseIndex = geometry.Positions.Count;

            //Add vertices
            geometry.Positions.Add(a);
            geometry.Positions.Add(b);
            geometry.Positions.Add(c);
            geometry.Positions.Add(d);

            //Add texture coordinates
            geometry.TextureCoordinates.Add(new Point(0, 1));
            geometry.TextureCoordinates.Add(new Point(1, 1));
            geometry.TextureCoordinates.Add(new Point(1, 0));
            geometry.TextureCoordinates.Add(new Point(0, 0));

            //Add normals
            geometry.Normals.Add(normal);
            geometry.Normals.Add(normal);
            geometry.Normals.Add(normal);
            geometry.Normals.Add(normal);

            //Add indices
            geometry.TriangleIndices.Add(baseIndex + 0);
            geometry.TriangleIndices.Add(baseIndex + 1);
            geometry.TriangleIndices.Add(baseIndex + 2);
            geometry.TriangleIndices.Add(baseIndex + 2);
            geometry.TriangleIndices.Add(baseIndex + 3);
            geometry.TriangleIndices.Add(baseIndex + 0);
        }
    }
}