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
    /// Interaction logic for PrelimsDialog.xaml
    /// </summary>
    public partial class PrelimsDialog : Window
    {
        public PrelimsDialog()
        {
            InitializeComponent();

            m_teams = PrelimsService.GetTeams();
            PadTeams();

            m_dataGrid.DataContext = this;
        }

        public List<TeamInfo> ListOfTeams
        {
            get { return m_teams; }
            set { m_teams = value; }
        }

        private void PadTeams()
        {
            while (m_teams.Count <= 100)
            {
                m_teams.Add(new TeamInfo());
            }
        }

        private List<TeamInfo> UnpadTeams()
        {
            var realTeams = new List<TeamInfo>();
            foreach (var team in m_teams)
                if (team.TeamId != 0)
                    realTeams.Add(team);
            return realTeams;
        }

        private void UpdateServer(object sender, RoutedEventArgs e)
        {
            List<TeamInfo> realTeams = UnpadTeams();
            if (realTeams.Count > 0)
                PrelimsService.SetTeams(realTeams);
        }

        private List<TeamInfo> m_teams;
    }
}
