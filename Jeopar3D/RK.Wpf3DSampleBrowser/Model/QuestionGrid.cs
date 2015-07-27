using Shenoy.Question.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RK.Wpf3DSampleBrowser.Model
{
    class QuestionGrid
    {
        public const int NUM_TOPICS = 5;
        public const int NUM_TYPES = 4;
        public const int NUM_POINTS = 3;

        String[] m_topics = {
                            "Techknowledgy\n",
                            "Bhag Bhag\nGK Bose\n",
                            "Pop goes the\nculture\n",
                            "Global\nVariables\n",
                            "Can you guess\nmy name?\n",
                            };
        String[] m_types = {
                            "What's Puzzing You...",
                            "tl;dr",
                            "P/A/R/V",
                            "Connect the dots"
                            };
        String[] m_typeAbbr = {
                            "(Puzz)",
                            "(TLDR)",
                            "(PARV)",
                            "(Conn)"
                            };
        Brush[] m_brushes = {
                            Brushes.LawnGreen,
                            Brushes.WhiteSmoke,
                            Brushes.Yellow,
                            Brushes.LightSkyBlue,
                            };

        private Dictionary<Tuple<int, int, int>, int> m_loc2qid = new Dictionary<Tuple<int, int, int>, int>();

        public QuestionGrid()
        {
            Questions.Load("questions.xml");

            InitQuestionMap();
        }

        private void InitQuestionMap()
        {
            int qid = 1;
            //Puzzles
            m_loc2qid.Add(new Tuple<int, int, int>(0, 0, 2), qid++); //Tech
            m_loc2qid.Add(new Tuple<int, int, int>(1, 0, 2), qid++); //Geek
            m_loc2qid.Add(new Tuple<int, int, int>(2, 0, 2), qid++); //Pop
            m_loc2qid.Add(new Tuple<int, int, int>(3, 0, 2), qid++); //G/L
            m_loc2qid.Add(new Tuple<int, int, int>(4, 0, 2), qid++); //Who

            //Text
            m_loc2qid.Add(new Tuple<int, int, int>(0, 1, 0), qid++); //Tech
            m_loc2qid.Add(new Tuple<int, int, int>(0, 1, 1), qid++); //Tech
            m_loc2qid.Add(new Tuple<int, int, int>(0, 1, 2), qid++); //Tech
            m_loc2qid.Add(new Tuple<int, int, int>(1, 1, 0), qid++); //Geek
            m_loc2qid.Add(new Tuple<int, int, int>(1, 1, 1), qid++); //Geek
            m_loc2qid.Add(new Tuple<int, int, int>(1, 1, 2), qid++); //Geek
            m_loc2qid.Add(new Tuple<int, int, int>(2, 1, 0), qid++); //Pop
            m_loc2qid.Add(new Tuple<int, int, int>(2, 1, 1), qid++); //Pop
            m_loc2qid.Add(new Tuple<int, int, int>(2, 1, 2), qid++); //Pop
            m_loc2qid.Add(new Tuple<int, int, int>(3, 1, 0), qid++); //G/L
            m_loc2qid.Add(new Tuple<int, int, int>(3, 1, 1), qid++); //G/L
            m_loc2qid.Add(new Tuple<int, int, int>(3, 1, 2), qid++); //G/L
            m_loc2qid.Add(new Tuple<int, int, int>(4, 1, 0), qid++); //Who
            m_loc2qid.Add(new Tuple<int, int, int>(4, 1, 1), qid++); //Who
            m_loc2qid.Add(new Tuple<int, int, int>(4, 1, 2), qid++); //Who
            
            //Parv
            m_loc2qid.Add(new Tuple<int, int, int>(0, 2, 0), qid++); //Tech
            m_loc2qid.Add(new Tuple<int, int, int>(0, 2, 1), qid++); //Tech
            m_loc2qid.Add(new Tuple<int, int, int>(1, 2, 0), qid++); //Geek
            m_loc2qid.Add(new Tuple<int, int, int>(1, 2, 1), qid++); //Geek
            m_loc2qid.Add(new Tuple<int, int, int>(2, 2, 0), qid++); //Pop
            m_loc2qid.Add(new Tuple<int, int, int>(2, 2, 1), qid++); //Pop
            m_loc2qid.Add(new Tuple<int, int, int>(3, 2, 2), qid++); //G/L
            m_loc2qid.Add(new Tuple<int, int, int>(4, 2, 0), qid++); //Who
            m_loc2qid.Add(new Tuple<int, int, int>(4, 2, 1), qid++); //Who

            //Connects
            m_loc2qid.Add(new Tuple<int, int, int>(0, 3, 2), qid++); //Tech
            m_loc2qid.Add(new Tuple<int, int, int>(1, 3, 0), qid++); //Geek
            m_loc2qid.Add(new Tuple<int, int, int>(2, 3, 1), qid++); //Pop
            m_loc2qid.Add(new Tuple<int, int, int>(3, 3, 1), qid++); //G/L
            m_loc2qid.Add(new Tuple<int, int, int>(4, 3, 2), qid++); //Who
        }

        public Question GetQuestion(int itopic, int itype, int ipoints)
        {
            int qid = GetQuestionId(itopic, itype, ipoints);
            if (qid < 0)
                return null;
            return Questions.Get(qid);
        }

        public int GetQuestionId(int itopic, int itype, int ipoints)
        {
            Tuple<int, int, int> loc = new Tuple<int,int,int>(itopic, itype, ipoints);
            if (!m_loc2qid.ContainsKey(loc))
                return -1;
            return m_loc2qid[loc];
        }

        public string QText(int itopic, int itype, int ipoints)
        {
            return m_topics[itopic] + m_typeAbbr[itype] + (++ipoints * 10);
        }

        public Brush QColor(int itopic, int itype, int ipoints)
        {
            return m_brushes[itype];
        }
    }
}
