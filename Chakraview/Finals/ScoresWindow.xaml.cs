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
    /// Interaction logic for ScoresWindow.xaml
    /// </summary>
    public partial class ScoresWindow : Window
    {
        public ScoresWindow()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void UpdateUI()
        {
            try
            {
                List<TeamInfo> teams = FinalsService.GetTeams();
                UpdateUI(teams[0], m_txtTeam1Name, m_txtTeam1Score);
                UpdateUI(teams[1], m_txtTeam2Name, m_txtTeam2Score);
                UpdateUI(teams[2], m_txtTeam3Name, m_txtTeam3Score);
                UpdateUI(teams[3], m_txtTeam4Name, m_txtTeam4Score);
                UpdateUI(teams[4], m_txtTeam5Name, m_txtTeam5Score);
                UpdateUI(teams[5], m_txtTeam6Name, m_txtTeam6Score);
            }
            catch
            {

            }
        }

        private void UpdateUI(TeamInfo teamInfo, TextBlock m_txtTeam1Name, TextBlock m_txtTeam1Score)
        {
            m_txtTeam1Name.Text = String.Format("{0}\n{1}", teamInfo.FirstPersonName, teamInfo.SecondPersonName);
            m_txtTeam1Score.Text = teamInfo.Score.ToString();
        }
    }
}
