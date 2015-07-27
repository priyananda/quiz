using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shenoy.Question.Model
{
    public class Connection
    {
        public Connection(Answer src, Clue dest)
        {
            m_source = src;
            m_target = dest;
        }
        public Question SourceQ { get { return m_source.Q; } }
        public Question TargetQ { get { return m_target.Q; } }
        public Answer Source { get { return m_source; } }
        public Clue Target { get { return m_target; } }

        private Answer m_source;
        private Clue m_target;
    }
}