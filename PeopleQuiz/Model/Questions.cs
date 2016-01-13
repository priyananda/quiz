using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Shenoy.Quiz.Model
{
    public class Questions
    {
        public Question Get(int i)
        {
            return m_questions[i];
        }
        public void Load(string filename)
        {
            XElement questions = XElement.Load(filename);
            var qkids = questions.Elements("question");
            m_questions = new Question[qkids.Count() + 1];
            int i = 1;
            foreach (var qelem in qkids)
            {
                m_questions[i] = Question.Create(qelem);
                m_questions[i].Answered += new Action<Question>(OnQuestionAnswered);
                if (!m_questions[i++].AllPlay)
                    ++m_csingleplay;
            }
            m_chalfway = m_csingleplay / 2;
            Clue.ResolveConnections();
        }
        public IEnumerable<Question> QList
        {
            get { return m_questions; }
        }
        public int Count
        {
            get { return m_questions.Length; }
        }
        public int FirstInSet
        {
            get { return m_setIndex * QUESTIONS_PER_SET + 1; }
        }
        public int LastInSet
        {
            get { return FirstInSet + QUESTIONS_PER_SET - 1; }
        }
        public void AdvanceSet()
        {
            m_setIndex++;
        }
        public int CurrentSet
        {
            get { return m_setIndex; }
        }

        public event Action DirectionChange;
        public event Action<Question> QuestionAnswered;
        public const int QUESTIONS_PER_SET = 16;

        public int CountInRange(int qidStart, int qidEnd, Predicate<Question> FCheck)
        {
            int c = 0;
            for(int qid = qidStart; qid <= qidEnd; ++qid )
            {
                Question question = Get(qid);
                if (question != null && FCheck(question))
                    ++c;
            }
            return c;
        }
        public int CountOpenWomen()
        {
            return CountInRange(FirstInSet, LastInSet,
                (q) => (!q.IsAnswered && PersonTraits.GenderOf(q.Person) == GenderType.Female));
        }

        public int CountOpenEvenOdd(bool fEven)
        {
            return CountInRange(FirstInSet, LastInSet,
                (q) => (!q.IsAnswered && (q.Id % 2 == (fEven ? 0 : 1))));
        }

        private void OnQuestionAnswered(Question question)
        {
            if (!question.AllPlay)
                m_csingleplay--;
            if (m_csingleplay == m_chalfway)
                if (DirectionChange != null)
                    DirectionChange();
            if (QuestionAnswered != null)
                QuestionAnswered(question);
        }

        private Question[] m_questions;
        private int m_csingleplay;
        private int m_chalfway;
        private int m_setIndex = 0;
    }
}
