using Shenoy.Quiz.Backend;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Shenoy.Quiz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Round1Start(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Round 1");
        }

        private void Round2Start(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Round 2");
        }

        private void Round3Start(object sender, RoutedEventArgs e)
        {
            new Round3Window().ShowDialog();
        }

        private void Round4Start(object sender, RoutedEventArgs e)
        {
            new Round4Window().ShowDialog();
        }

        private void Round5Start(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Round 5");
        }
        private void Scores(object sender, RoutedEventArgs e)
        {
            new ScoresWindow().ShowDialog();
        }
    }
}
