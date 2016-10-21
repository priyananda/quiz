using Shenoy.Quiz.Connector;
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

namespace Shenoy.Quiz
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowPrelims(object sender, RoutedEventArgs e)
        {
            //new PrelimsDialog().ShowDialog();
        }

        private void ShowFinals(object sender, RoutedEventArgs e)
        {
            new FinalsDialog().ShowDialog();
        }
    }
}
