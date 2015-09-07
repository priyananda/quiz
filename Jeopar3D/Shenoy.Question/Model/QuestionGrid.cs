using Shenoy.Question.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;

namespace Shenoy.Question.Model
{
    public struct QuestionKey
    {
        public int Topic;
        public int Type;
        public int Points;

        public QuestionKey(int _topic, int _type, int _points)
        {
            this.Topic  = _topic;
            this.Type   = _type;
            this.Points = _points;
        }

        public override bool Equals(object obj)
        {
            var otherKey = (QuestionKey)obj;
            return otherKey.Topic == Topic && otherKey.Type == Type && otherKey.Points == Points;
        }

        public override int GetHashCode()
        {
            return Topic * 100 + Type * 10 + Points;
        }
    }

    class TypeDesc
    {
        public string Name;
        public string Abbr;

        public TypeDesc(string _name, string _abbr)
        {
            this.Name = _name;
            this.Abbr = _abbr;
        }
    }

    public class QuestionGrid
    {
        public int NumTopics
        {
            get { return m_topics.Count; }
        }

        public int NumTypes
        {
            get { return m_types.Count; }
        }

        public int NumPoints
        {
            get { return 3; }
        }

        public QuestionGrid()
        {
            Questions.Load("questions.xml");

            InitQuestionMap();
        }

        public Question GetQuestion(QuestionKey key)
        {
            int qid = GetQuestionId(key);
            if (qid < 0)
                return null;
            return Questions.Get(qid);
        }

        public int GetQuestionId(QuestionKey key)
        {
            if (!m_qkey2qid.ContainsKey(key))
                return 1;
            return m_qkey2qid[key];
        }

        public string QText(QuestionKey key)
        {
            //return m_topics[key.Topic] + m_types[key.Type].Abbr + ((key.Points + 1) * 10);
            bool fAllPlay = GetQuestion(key).AllPlay;
            return ((key.Points + 1) * 10).ToString() +
                (fAllPlay ? "\n[AP]" : "");
        }
         
        public string TopicText(int topic)
        {
            return m_topics[topic];
        }

        public string TypeText(int topic)
        {
            return m_types[topic].Name;
        }

        public Brush QColor(QuestionKey key)
        {
            return m_brushes[key.Type];
        }

        private void InitQuestionMap()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("joepardy.xml");

            LoadTopics(doc);
            LoadTypes(doc);
            LoadMapping(doc);
        }

        private void LoadMapping(XmlDocument doc)
        {
            foreach (var mappingNode in doc.DocumentElement.SelectNodes("mapping"))
            {
                XmlElement mappingElem = mappingNode as XmlElement;
                int topic = Int32.Parse(mappingElem.GetAttribute("topic"));
                int type = Int32.Parse(mappingElem.GetAttribute("type"));
                int points = Int32.Parse(mappingElem.GetAttribute("points"));
                int qid = Int32.Parse(mappingElem.GetAttribute("question"));

                m_qkey2qid.Add(new QuestionKey(topic, type, points), qid);
            }
        }

        private void LoadTypes(XmlDocument doc)
        {
            var typesElem = doc.DocumentElement.SelectSingleNode("types");
            foreach (var typeNode in typesElem.SelectNodes("type"))
            {
                XmlElement typeElem = typeNode as XmlElement;
                m_types.Add(new TypeDesc(FixNewLines(typeElem.GetAttribute("name")), typeElem.GetAttribute("abbr")));
            }
        }

        private void LoadTopics(XmlDocument doc)
        {
            var topicsElem = doc.DocumentElement.SelectSingleNode("topics");
            foreach (var topicNode in topicsElem.SelectNodes("topic"))
            {
                XmlElement topicElem = topicNode as XmlElement;
                m_topics.Add(FixNewLines(topicElem.GetAttribute("name")));
            }
        }

        static string FixNewLines(string s)
        {
            return s.Replace("\\n", "\n");
        }

        private List<string> m_topics = new List<string>();
        private List<TypeDesc> m_types = new List<TypeDesc>();
        private Dictionary<QuestionKey, int> m_qkey2qid = new Dictionary<QuestionKey, int>();
        private Brush[] m_brushes =
        {
            Brushes.Goldenrod,
            Brushes.WhiteSmoke,
            Brushes.BlanchedAlmond,
            Brushes.LightSkyBlue,
        };
    }
}
