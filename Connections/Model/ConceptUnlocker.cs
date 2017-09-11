using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnQuiz.Model
{
    class ConceptUnlocker
    {
        public static void OnQuestionAnswered(Question q)
        {
            if (m_mapq2t == null)
                PreProcess();
            if (!m_mapq2t.ContainsKey(q.Id))
                return;
            int iconcept = m_mapq2t[q.Id];
            if (m_topicCounts[iconcept] > 0)
                m_topicCounts[iconcept]--;
            if (m_topicCounts[iconcept] <= 0)
                Questions.Get(iconcept).AnswerQuestion();
        }

        private static void PreProcess()
        {
            m_mapq2t = new Dictionary<int, int>();
            m_mapq2t.Add(10, 1); m_mapq2t.Add(11, 1); m_mapq2t.Add(12, 1); m_mapq2t.Add(13, 2); m_mapq2t.Add(14, 2);
            m_mapq2t.Add(15, 2); m_mapq2t.Add(16, 2); m_mapq2t.Add(17, 1); m_mapq2t.Add(18, 3); m_mapq2t.Add(19, 3);
            m_mapq2t.Add(20, 3); m_mapq2t.Add(21, 4); m_mapq2t.Add(22, 4); m_mapq2t.Add(23, 5); m_mapq2t.Add(24, 5);
            m_mapq2t.Add(25, 5); m_mapq2t.Add(26, 5); m_mapq2t.Add(27, 5); m_mapq2t.Add(28, 5); m_mapq2t.Add(29, 5);
            m_mapq2t.Add(30, 5); m_mapq2t.Add(31, 6); m_mapq2t.Add(32, 6); m_mapq2t.Add(33, 6); m_mapq2t.Add(34, 6);
            m_mapq2t.Add(35, 7); m_mapq2t.Add(36, 6); m_mapq2t.Add(37, 6); m_mapq2t.Add(38, 7); m_mapq2t.Add(39, 7);
            m_mapq2t.Add(40, 7); m_mapq2t.Add(41, 7); m_mapq2t.Add(42, 7); m_mapq2t.Add(43, 8); m_mapq2t.Add(44, 8);
            m_mapq2t.Add(45, 8); m_mapq2t.Add(46, 1);

            foreach (var item in m_mapq2t)
            {
                m_topicCounts[item.Value]++;
            }
        }

        private static Dictionary<int, int> m_mapq2t;
        private static int[] m_topicCounts = new int[9];
    }
}
