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
    /// Interaction logic for WpfD3DImageHost.xaml
    /// </summary>
    public partial class WpfD3DImageHost : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfD3DImageHost" /> class.
        /// </summary>
        public WpfD3DImageHost()
        {
            InitializeComponent();
        }

        public Scene Scene
        {
            get { return this.m_direct3DImage.Scene; }
            set { this.m_direct3DImage.Scene = value; }
        }

        public Camera Camera
        {
            get { return this.m_direct3DImage.Camera; }
            set { this.m_direct3DImage.Camera = value; }
        }
    }
}
