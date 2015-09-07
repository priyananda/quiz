using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Linq;

namespace Shenoy.Question.Model
{
    public class Questions
    {
        public static Question Get(int i)
        {
            return m_questions[i];
        }
        public static void Load(string filename)
        {
            XElement questions = XElement.Load(filename);
            var qkids = questions.Elements("question");
            m_questions = new Question[qkids.Count() + 1];
            int i = 1;
            foreach (var qelem in qkids)
            {
                m_questions[i] = QuestionFactory.Parse(qelem);
                m_questions[i].Answered += new Action<Question>(OnQuestionAnswered);
                if (!m_questions[i++].AllPlay)
                    ++m_csingleplay;
            }
            m_chalfway = m_csingleplay / 2;
            Clue.ResolveConnections();
        }

        public static IEnumerable<Question> QList
        {
            get { return m_questions; }
        }
        public static int Count
        {
            get { return m_questions.Length; }
        }
        public static Color ColorForQuestion(int qid)
        {
            float x = 0.1f + (qid) / (50.0f + m_questions.Length);
            return Color.FromScRgb(1.0f, x, x, x);
        }
        private static void OnQuestionAnswered(Question obj)
        {
            if (!obj.AllPlay)
                m_csingleplay--;
            if (m_csingleplay == m_chalfway)
                if (DirectionChange != null)
                    DirectionChange();
        }
        private static Question[] m_questions;
        private static int m_csingleplay;
        private static int m_chalfway;
        public static event Action DirectionChange;
    }
}
