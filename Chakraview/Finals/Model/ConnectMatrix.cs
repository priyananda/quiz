using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Model
{
    class ConnectMatrix
    {
        public ConnectMatrix(string[] items)
        {
            int ilist = 0;
            for (int igroup = 0; igroup < 4; ++igroup)
                for (int icol = 0; icol < 4; ++icol)
                {
                    m_clueGroups[items[ilist++]] = igroup;
                }
            GenerateVisualGrid();
        }

        public Tuple<String, bool> GetItem(int irow, int icol)
        {
            return new Tuple<string, bool>(m_visualGrid[irow, icol], irow < m_completedRows);
        }

        public bool Check(string[] row)
        {
            int group = m_clueGroups[row[0]];
            for (int i = 1; i < row.Length; ++i)
                if (m_clueGroups[row[i]] != group)
                    return false;
            AddCompletedGroup(group);
            return true;
        }

        private void AddCompletedGroup(int group)
        {
            m_foundGroup[group] = true;
            m_completedRows++;
            if (m_completedRows == 3)
            {
                m_completedRows = 4;
                for (int igroup = 0; igroup < m_foundGroup.Length; ++igroup)
                    m_foundGroup[igroup] = true;
            }
            GenerateVisualGrid();
        }

        private void GenerateVisualGrid()
        {
            // first arrange all the found groups
            int irow = 0;
            for (int igroup = 0; igroup < m_foundGroup.Length; ++igroup)
            {
                if (!m_foundGroup[igroup])
                    continue;
                int icol = 0;
                foreach(string key in m_clueGroups.Keys)
                {
                    if (m_clueGroups[key] == igroup)
                        m_visualGrid[irow, icol++] = key;
                }
                ++irow;
            }

            if (irow == 4)
                return;

            // Shuffle the rest
            List<string> jumble = new List<string>();
            for (int igroup = 0; igroup < m_foundGroup.Length; ++igroup)
            {
                if (m_foundGroup[igroup])
                    continue;
                foreach (string key in m_clueGroups.Keys)
                {
                    if (m_clueGroups[key] == igroup)
                        jumble.Add(key);
                }
            }
            Shuffle(jumble);

            int ilist = 0;
            for (; irow < 4; ++irow)
                for (int icol = 0; icol < 4; ++icol)
                    m_visualGrid[irow, icol] = jumble[ilist++];
        }

        public int Points
        {
            get { return m_completedRows * 5; }
        }

        private void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private string[,] m_visualGrid = new string[4, 4];
        private Dictionary<string, int> m_clueGroups = new Dictionary<string, int>();
        private bool[] m_foundGroup = new bool[4];
        private int m_completedRows = 0;
    }
}
