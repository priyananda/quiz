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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Shenoy.Quiz
{
    /// <summary>
    /// Interaction logic for Round3Window.xaml
    /// </summary>
    public partial class Round3Window : Window
    {
        public Round3Window()
        {
            InitializeComponent();

            m_clueImages = new Image[]
            {
                m_clueImage1,
                m_clueImage2,
                m_clueImage3,
                m_clueImage4,
                m_clueImage5,
            };
            m_timer.Interval = TimeSpan.FromMilliseconds(500);
            m_timer.Tick += OnTimerTick;

            InitState();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            m_progress.Value = m_progress.Value + 1;
            if (m_progress.Value >= m_progress.Maximum)
            {
                StopTimer(sender, null);
                return;
            }
            int points = 25;
            if (m_progress.Value > 1)
                m_clueImages[0].Visibility = Visibility.Visible;
            if (m_progress.Value > 25)
            {
                m_clueImages[1].Visibility = Visibility.Visible;
                points -= 5;
            }
            if (m_progress.Value > 50)
            {
                m_clueImages[2].Visibility = Visibility.Visible;
                points -= 5;
            }
            if (m_progress.Value > 75)
            {
                m_clueImages[3].Visibility = Visibility.Visible;
                points -= 5;
            }
            if (m_progress.Value > 100)
            {
                m_clueImages[4].Visibility = Visibility.Visible;
                points -= 5;
            }
            m_txtPoints.Content = points.ToString();
        }

        private void InitState()
        {
            foreach (var image in m_clueImages)
                image.Visibility = Visibility.Hidden;
            m_progress.Value = 0;
            m_txtPoints.Content = "25";
        }

        private void ShowSet1(object sender, MouseEventArgs me)
        {
            ShowSet(0, sender as Image);
        }
        private void ShowSet2(object sender, MouseEventArgs me)
        {
            ShowSet(1, sender as Image);
        }
        private void ShowSet3(object sender, MouseEventArgs me)
        {
            ShowSet(2, sender as Image);
        }
        private void ShowSet4(object sender, MouseEventArgs me)
        {
            ShowSet(3, sender as Image);
        }
        private void ShowSet5(object sender, MouseEventArgs me)
        {
            ShowSet(4, sender as Image);
        }

        private void ShowSet6(object sender, MouseEventArgs me)
        {
            ShowSet(5, sender as Image);
        }

        private void ShowSet(int v, Image image)
        {
            if (image != null)
                image.Visibility = Visibility.Hidden;
            m_currentSet = v;
            InitState();
        }

        private void StartTimer(object sender, RoutedEventArgs e)
        {
            if (m_currentSet < 0)
                return;
            if (!m_timer.IsEnabled)
            {
                m_timer.IsEnabled = true;
                m_timer.Start();
            }
        }

        private void StopTimer(object sender, RoutedEventArgs e)
        {
            if (m_timer.IsEnabled)
            {
                m_timer.IsEnabled = false;
                m_timer.Stop();
            }
        }

        private int m_currentSet = -1;
        private Image[] m_clueImages;
        private DispatcherTimer m_timer = new DispatcherTimer();
    }
}
