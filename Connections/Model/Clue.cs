using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConnQuiz.Model
{
    struct ConnectionInfo
    {
        public int connSrc;
        public int points;
    }

    class Clue : ObjectWithSlide
    {
        public static Clue Create(Question q, XElement elem)
        {
            int slideid = Int32.Parse(elem.Attribute("slideid").Value);
            var ret = new Clue(slideid, q);
            if (elem.Attribute("connection") != null)
            {
                ConnectionInfo info = new ConnectionInfo();
                info.connSrc = Int32.Parse(elem.Attribute("connection").Value);
                if (elem.Attribute("points") != null)
                    info.points = Int32.Parse(elem.Attribute("points").Value);
                else
                    info.points = -1;
                m_unresolvedConnections[ret] = info;
            }
            ret.LoadVideoData(elem);
            return ret;
        }
        public static void ResolveConnections()
        {
            if (m_unresolvedConnections == null)
                return;
            foreach(var clue in m_unresolvedConnections.Keys)
            {
                int qid = m_unresolvedConnections[clue].connSrc;
                Connection conn = new Connection(Questions.Get(qid).Ans, clue, m_unresolvedConnections[clue].points);
                clue.m_connection = conn;
            }
            m_unresolvedConnections = null;
        }
        public Clue(int slideid, Question q) : base(slideid)
        {
            m_question = q;
        }
        public bool Unlocked
        {
            get
            {
                return m_connection == null || m_connection.SourceQ.IsAnswered;
            }
        }
        public Question Q
        {
            get { return m_question; }
        }
        public Connection Source
        {
            get { return m_connection; }
        }
        public override int SlideId
        {
            get
            {
                if (this.Unlocked && m_connection != null)
                    return m_connection.Source.SlideId;
                return base.SlideId;
            }
        }
        private Question m_question;
        private Connection m_connection;

        private static Dictionary<Clue, ConnectionInfo> m_unresolvedConnections = new Dictionary<Clue, ConnectionInfo>();
    }
}