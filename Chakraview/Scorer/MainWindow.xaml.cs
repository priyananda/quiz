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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //TeamInfo teamInfo = new TeamInfo();
            //teamInfo.TeamId = 101;
            //teamInfo.FirstPersonName = "Heera";
            //teamInfo.SecondPersonName = "Panna";
            //teamInfo.IsFinalist = false;

            //List<TeamInfo> teams = new List<TeamInfo>();
            //teams.Add(teamInfo);

            //Server.Push(teams);

            var teams = PrelimsService.GetTeams();
            int c = teams.Count;
        }
    }
}
