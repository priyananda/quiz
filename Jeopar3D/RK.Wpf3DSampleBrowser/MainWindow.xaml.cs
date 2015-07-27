using Shenoy.Question.Model;
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

namespace RK.Wpf3DSampleBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Teams.Get(0).Points = (int)teamScore1.Value;
            Teams.Get(1).Points = (int)teamScore2.Value;
            Teams.Get(2).Points = (int)teamScore3.Value;
            Teams.Get(3).Points = (int)teamScore4.Value;
            Teams.Get(4).Points = (int)teamScore5.Value;
            Teams.Get(5).Points = (int)teamScore6.Value;
            Log.SaveState();
        }
    }
}
