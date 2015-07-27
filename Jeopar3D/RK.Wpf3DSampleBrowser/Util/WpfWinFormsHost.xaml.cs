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
    /// Interaction logic for WpfWinFormsHost.xaml
    /// </summary>
    public partial class WpfWinFormsHost : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfWinFormsHost" /> class.
        /// </summary>
        public WpfWinFormsHost()
        {
            InitializeComponent();

            m_viewport3D.CyclicRendering = true;
            m_viewport3D.IsMovementEnabled = true;
        }

        public Scene Scene
        {
            get { return this.m_viewport3D.Scene; }
            set { this.m_viewport3D.Scene = value; }
        }

        public Camera Camera
        {
            get { return this.m_viewport3D.Camera; }
            set { m_viewport3D.Camera = value; }
        }
    }
}
