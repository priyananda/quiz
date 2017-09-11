using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnQuiz.Model
{
    class Connection
    {
        public Connection(Answer src, Clue dest, int points = -1)
        {
            m_source = src;
            m_target = dest;
            if (points > 0)
                m_points = points;
        }
        public Question SourceQ { get { return m_source.Q; } }
        public Question TargetQ { get { return m_target.Q; } }
        public Answer Source { get { return m_source; } }
        public Clue Target { get { return m_target; } }
        public int Points { get { return m_points; } }

        private Answer m_source;
        private Clue m_target;
        private int m_points = Constants.PointsForUnknownClue;
    }
}