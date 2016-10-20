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
            InitState();
            RenderVisualGrid();
        }

        private void InitState()
        {
            m_currentMatrix = ConnectMatrixRepository.GetMatrix(0);
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
            InitState();
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

        private void UnCheckAll()
        {
            for (int irow = 0; irow < 4; ++irow)
                for (int icol = 0; icol < 4; ++icol)
                {
                    m_buttons[irow, icol].IsChecked = false;
                }
        }

        private ConnectMatrix m_currentMatrix;
        private ToggleButton[,] m_buttons = new ToggleButton[4,4];
        private Brush[] m_brushes;
        private HashSet<string> m_selectedItems = new HashSet<string>();
    }
}