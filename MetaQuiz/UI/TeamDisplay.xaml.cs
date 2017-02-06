using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Shenoy.Quiz.Model;

namespace Shenoy.Quiz.UI
{
    /// <summary>
    /// Interaction logic for TeamDisplay.xaml
    /// </summary>
    public partial class TeamDisplay : UserControl
    {
        public TeamDisplay()
        {
            InitializeComponent();
        }

        public int TeamId
        {
            set
            {
                m_team = Teams.Get(value);
                m_team.PointsChanged += new Action<Team>(OnUpdate);
                OnUpdate(m_team);
            }
        }

        void OnUpdate(Team obj)
        {
            txtName.Text = obj.Name;
            txtScore.Text = obj.Points.ToString();
        }

        private void Plus10Click(object sender, RoutedEventArgs e)
        {
            if (m_team != null)
                m_team.AddPoints(10);
        }
        private void Plus5Click(object sender, RoutedEventArgs e)
        {
            if (m_team != null)
                m_team.AddPoints(5);
        }
        private void Minus5Click(object sender, RoutedEventArgs e)
        {
            if (m_team != null)
                m_team.AddPoints(-5);
        }

        Team m_team;
    }
}
