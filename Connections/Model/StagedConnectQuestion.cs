using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConnQuiz.Model
{
    class StagedConnectQuestion : Question
    {
        public StagedConnectQuestion(int id): base(id, QuestionType.StagedConnect)
        {
        }
        public override int Points
        {
            get
            {
                return PointsForStage(m_currentSet);
            }
        }
        public int PointsForStage(int stage)
        {
            int points = m_rgPoints[stage];
            for (int i = 0; i <= stage; ++i)
                foreach (var clue in m_rgClueSets[i])
                    if (!clue.Unlocked)
                        points += clue.Source.SourceQ.Points;
            return points;
            
        }
        public override void Advance()
        {
            if (!this.IsAnswered)
            {
                if (m_currentSet < m_rgClueSets.Length - 1)
                    m_currentSet++;
                else
                    this.AnswerQuestion();
            }
        }
        public override void Load(XElement elem)
        {
            base.Load(elem);
            var csets = elem.Elements("clueset");
            m_rgClueSets = new ClueSet[csets.Count()];
            m_rgPoints = new int[csets.Count()];
            int i = 0;
            foreach (var csetelem in csets)
            {
                m_rgClueSets[i] = new ClueSet();
                m_rgClueSets[i++].Load(this, csetelem);
            }
            if (elem.Attribute("points") != null)
            {
                var sPoints = elem.Attribute("points").Value.Split(',');
                for (i = 0; i < m_rgPoints.Length; ++i)
                    m_rgPoints[i] = Int32.Parse(sPoints[i]);
            }
            else
            {
                for (i = 0; i < m_rgPoints.Length; ++i)
                    m_rgPoints[i] = Constants.PointsForSimpleQ * (m_rgPoints.Length - i);
            }
        }
        public ClueSet[] ClueSets
        {
            get
            {
                return m_rgClueSets;
            }
        }
        private ClueSet[] m_rgClueSets;
        private int[] m_rgPoints;
        private int m_currentSet;
    }
}