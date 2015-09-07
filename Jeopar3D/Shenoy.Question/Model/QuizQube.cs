using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml;

namespace Shenoy.Question.Model
{
    public class QuestionCube
    {
        public Point3D Location;
        public Brush Background;
        public String Text;
    }

    public class HeaderCube
    {
        public Point3D Location;
        public Brush BackgroundColor;
        public BitmapImage BackgroundImage;
        public String Text;
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

    class TopicDesc
    {
        public string Name;
        public string Logo;

        public TopicDesc(string _name, string _logo)
        {
            this.Name = _name;
            this.Logo = _logo;
        }
    }

    public class QuizQube
    {
        public QuizQube()
        {
            Questions.Load("questions.xml");

            InitQuestionMap();
        }

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

        public QuestionCube GetQuestionCube(int qid)
        {
            Question question = Questions.Get(qid);
            if (question == null)
                return null;

            int itype = TypeFromQid(qid);
            int iRowPos = RowPosFromQid(qid);

            var cube = new QuestionCube();
            cube.Background = m_brushes[itype];
            cube.Text = qid.ToString() + (question.AllPlay ? "\n[AP]" : "");
            cube.Location = new Point3D();
            cube.Location.Y = 20 - ((yStart + itype) * yZoom);
            switch(iRowPos)
            {
                case 0: case 6: case 7: cube.Location.X = (xStart + 0) * xZoom; break;
                case 1: case 5:         cube.Location.X = (xStart + 1) * xZoom; break;
                case 2: case 3: case 4: cube.Location.X = (xStart + 2) * xZoom; break;
            }
            switch (iRowPos)
            {
                case 0: case 1: case 2: cube.Location.Z = (zStart + 0) * zZoom; break;
                case 3: case 7:         cube.Location.Z = (zStart + 1) * zZoom; break;
                case 4: case 5: case 6: cube.Location.Z = (zStart + 2) * zZoom; break;
            }
            return cube;
        }

        public HeaderCube GetTopicHeaderCube(int itopic)
        {
            QuestionCube quesCube = GetQuestionCube((itopic + 1) * 2);
            HeaderCube topicCube = new HeaderCube();
            topicCube.BackgroundImage = MediaManager.LoadImage(m_topics[itopic].Logo, false);
            topicCube.Location = quesCube.Location;
            topicCube.Location.Y += yZoom;
            return topicCube;
        }

        public HeaderCube GetTypeHeaderCube(int itype)
        {
            QuestionCube quesCube = GetQuestionCube(itype * 8 + 1);
            HeaderCube typeCube = new HeaderCube();
            typeCube.Text = m_types[itype].Name;
            typeCube.BackgroundColor = quesCube.Background;
            typeCube.Location = quesCube.Location;
            typeCube.Location.X -= xZoom;
            return typeCube;
        }

        private void InitQuestionMap()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("quizcube.xml");

            LoadTopics(doc);
            LoadTypes(doc);
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
                m_topics.Add(new TopicDesc(FixNewLines(topicElem.GetAttribute("name")), topicElem.GetAttribute("logo")));
            }
        }

        private static string FixNewLines(string s)
        {
            return s.Replace("\\n", "\n");
        }

        private int TypeFromQid(int qid)
        {
            --qid;
            return (qid / 8);
        }

        private int RowPosFromQid(int qid)
        {
            --qid;
            return (qid % 8);
        }

        private List<TopicDesc> m_topics = new List<TopicDesc>();
        private List<TypeDesc> m_types = new List<TypeDesc>();
        private Brush[] m_brushes =
        {
            Brushes.Goldenrod,
            Brushes.WhiteSmoke,
            Brushes.BlanchedAlmond,
            Brushes.LightSkyBlue,
        };
        const int xStart = -2, xZoom = +4;
        const int yStart = +1, yZoom = +4;
        const int zStart = -1, zZoom = -6;
    }
}
