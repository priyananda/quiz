using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Shenoy.Question.Model
{
    public class ConnectQuestion : Question
    {
        public ConnectQuestion(int id)
            : base(id, QuestionType.Connect)
        {
            m_clueset = new ClueSet();
        }
        public override int Points
        {
            get {
                int ret = m_Points;
                foreach (var clue in m_clueset)
                    if (!clue.Unlocked)
                        ret += clue.Source.SourceQ.Points;
                return ret;
            }
        }
        public override void Advance()
        {
            if (!this.IsAnswered)
                this.AnswerQuestion();
        }
        public override void Load(XElement elem)
        {
            base.Load(elem);
            m_clueset.Load(this, elem.Element("clueset"));
        }
        public ClueSet Clues
        {
            get { return m_clueset; }
        }
        private int CUnknownClues
        {
            get { return m_clueset.Count(x => !x.Unlocked); }
        }
        ClueSet m_clueset;
    }
}
