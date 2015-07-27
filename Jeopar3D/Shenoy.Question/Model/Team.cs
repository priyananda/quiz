using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shenoy.Question.Model
{
    public class Team
    {
        public event Action<Team> PointsChanged;
        public Team(int id)
        {
            m_id = id;
        }
        public string Name
        {
            get
            {
                if (m_id == 5)
                    return "Aswath";
                return String.Format("Team {0}", m_id + 1 );
            }
        }
        public int Points
        {
            get { return m_points; }
            set { m_points = value; }
        }
        public void AddPoints(int dp)
        {
            m_points += dp;
            if (PointsChanged != null)
                PointsChanged(this);
            Log.SaveState();
        }

        private int m_id;
        private int m_points;
    }

    public class Teams
    {
        static Teams()
        {
            for (int i = 0; i < Constants.NumTeams; ++i)
                rgTeams[i] = new Team(i);
        }
        public static Team Get(int i)
        {
            return rgTeams[i];
        }
        static Team[] rgTeams = new Team[Constants.NumTeams];
    }
}