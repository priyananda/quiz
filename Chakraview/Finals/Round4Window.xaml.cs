using Shenoy.Quiz.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for Round4Window.xaml
    /// </summary>
    public partial class Round4Window : Window
    {
        public Round4Window()
        {
            InitializeComponent();
            m_brushes = new Brush[]
            {
                Brushes.Goldenrod,
                Brushes.Ivory,
                Brushes.Lavender,
                Brushes.OldLace,
            };
            m_buttons = new ToggleButton[4, 4]
            {
                {m_btn00, m_btn01, m_btn02, m_btn03},
                {m_btn10, m_btn11, m_btn12, m_btn13},
                {m_btn20, m_btn21, m_btn22, m_btn23},
                {m_btn30, m_btn31, m_btn32, m_btn33},
            };
            DisableAll();
            m_timer.Interval = TimeSpan.FromMilliseconds(500);
            m_timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            m_progress.Value = m_progress.Value + 1;
            if (m_progress.Value >= m_progress.Maximum || m_currentMatrix.Points == 20)
            {
                StopTimer(sender, null);
                DisableAll();
                return;
            }
        }

        private void InitState(int v)
        {
            m_currentMatrix = ConnectMatrixRepository.GetMatrix(0);
            RenderVisualGrid();
            StartTimer(null, null);
            m_progress.Value = 0;
        }

        private void RenderVisualGrid()
        {
            for (int irow = 0; irow < 4; ++irow)
                for (int icol = 0; icol < 4; ++icol)
                {
                    var item = m_currentMatrix.GetItem(irow, icol);
                    m_buttons[irow, icol].Content = item.Item1;
                    if (item.Item2)
                    {
                        m_buttons[irow, icol].IsEnabled = false;
                        m_buttons[irow, icol].Background = m_brushes[irow];
                    }
                    else
                    {
                        m_buttons[irow, icol].IsEnabled = true;
                        m_buttons[irow, icol].Background = Brushes.AliceBlue;
                    }
                    m_buttons[irow, icol].IsChecked = false;
                }
            m_txtPoints.Content = m_currentMatrix.Points.ToString();
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
            InitState(v);
        }
        private void OnToggle(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (sender as ToggleButton);
            if (button.IsChecked.Value)
                m_selectedItems.Add(button.Content as string);
            else
                m_selectedItems.Remove(button.Content as string);

            if (m_selectedItems.Count >= 4)
            {
                if (m_currentMatrix.Check(m_selectedItems.ToArray()))
                    RenderVisualGrid();
                else
                    UnCheckAll();
                m_selectedItems.Clear();
            }
        }

        private void ForAll(Action<ToggleButton> action)
        {
            for (int irow = 0; irow < 4; ++irow)
                for (int icol = 0; icol < 4; ++icol)
                    action(m_buttons[irow, icol]);
        }

        private void UnCheckAll()
        {
            ForAll((x) => x.IsChecked = false);
        }

        private void DisableAll()
        {
            ForAll((x) => x.IsEnabled = false);
        }

        private void EnableAll()
        {
            ForAll((x) => x.IsEnabled = true);
        }

        private void StartTimer(object sender, RoutedEventArgs e)
        {
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

        private ConnectMatrix m_currentMatrix;
        private ToggleButton[,] m_buttons = new ToggleButton[4,4];
        private Brush[] m_brushes;
        private HashSet<string> m_selectedItems = new HashSet<string>();
        private DispatcherTimer m_timer = new DispatcherTimer();
    }
}