using Shenoy.Quiz.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Connector
{
    public class FinalsService
    {
        public static List<TeamInfo> GetTeams()
        {
            try
            {
                Server server = new Server(false);
                return server.Pull();
            }
            catch
            {
            }
            return new List<TeamInfo>();
        }

        public static void SetTeams(List<TeamInfo> teams)
        {
            try
            {
                Server server = new Server(false);
                server.Push(teams);
            }
            catch
            {
            }
        }
    }
}
