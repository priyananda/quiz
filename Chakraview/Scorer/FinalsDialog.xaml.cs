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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Shenoy.Quiz
{
    /// <summary>
    /// Interaction logic for FinalsDialog.xaml
    /// </summary>
    public partial class FinalsDialog : Window
    {
        public FinalsDialog()
        {
            InitializeComponent();
            InitTeams();
        }

        private void T1Plus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT1Score, DELTA);
        }
        private void T2Plus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT2Score, DELTA);
        }
        private void T3Plus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT3Score, DELTA);
        }
        private void T4Plus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT4Score, DELTA);
        }
        private void T5Plus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT5Score, DELTA);
        }
        private void T6Plus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT6Score, DELTA);
        }
        private void T1Minus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT1Score, -DELTA);
        }
        private void T2Minus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT2Score, -DELTA);
        }
        private void T3Minus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT3Score, -DELTA);
        }
        private void T4Minus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT4Score, -DELTA);
        }
        private void T5Minus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT5Score, -DELTA);
        }
        private void T6Minus5(object sender, RoutedEventArgs e)
        {
            ApplyDelta(m_txtT6Score, -DELTA);
        }

        private void ApplyDelta(TextBox txtBox, int delta)
        {
            int teamScore = 0;
            Int32.TryParse(txtBox.Text, out teamScore);
            teamScore += delta;
            txtBox.Text = teamScore.ToString();
        }

        private void DoUpdate(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateTeam(0, m_txtT1First.Text, m_txtT1Second.Text, m_txtT1Score.Text);
                UpdateTeam(1, m_txtT2First.Text, m_txtT2Second.Text, m_txtT2Score.Text);
                UpdateTeam(2, m_txtT3First.Text, m_txtT3Second.Text, m_txtT3Score.Text);
                UpdateTeam(3, m_txtT4First.Text, m_txtT4Second.Text, m_txtT4Score.Text);
                UpdateTeam(4, m_txtT5First.Text, m_txtT5Second.Text, m_txtT5Score.Text);
                UpdateTeam(5, m_txtT6First.Text, m_txtT6Second.Text, m_txtT6Score.Text);

                FinalsService.SetTeams(teams);
            }
            catch
            {
            }
        }

        private void InitTeams()
        {
            try
            {
                teams = FinalsService.GetTeams();
            }
            catch
            {
            }

            if (teams == null || teams.Count != 6)
            {
                teams = new List<TeamInfo>();
                for (int i = 0; i < 6; ++i)
                {
                    TeamInfo team = new TeamInfo();
                    team.TeamId = i + 1;
                    teams.Add(team);
                }
            }

            teams.Sort((ta, tb) => (int)ta.TeamId - (int)tb.TeamId);

            UpdateUI(0, m_txtT1First, m_txtT1Second, m_txtT1Score);
            UpdateUI(1, m_txtT2First, m_txtT2Second, m_txtT2Score);
            UpdateUI(2, m_txtT3First, m_txtT3Second, m_txtT3Score);
            UpdateUI(3, m_txtT4First, m_txtT4Second, m_txtT4Score);
            UpdateUI(4, m_txtT5First, m_txtT5Second, m_txtT5Score);
            UpdateUI(5, m_txtT6First, m_txtT6Second, m_txtT6Score);
        }

        private void UpdateUI(int v, TextBox txtFirst, TextBox txtSecond, TextBox txtScore)
        {
            txtFirst.Text = teams[v].FirstPersonName;
            txtSecond.Text = teams[v].SecondPersonName;
            txtScore.Text = teams[v].Score.ToString();
        }

        private void UpdateTeam(int v, string first, string second, string score)
        {
            teams[v].FirstPersonName = first;
            teams[v].SecondPersonName = second;

            long scoreVal = 0;
            Int64.TryParse(score, out scoreVal);
            teams[v].Score = scoreVal;
        }

        private const int DELTA = 5;
        private List<TeamInfo> teams;
    }
}
