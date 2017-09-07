using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnQuiz.Model
{
    class LongListQuestion : Question
    {
        public LongListQuestion(int id) : base(id, QuestionType.LongList)
        {
            m_clueset = new ClueSet();
        }
        public override void Advance()
        {
            if (!this.IsAnswered)
                this.AnswerQuestion();
        }
        public ClueSet Clues
        {
            get { return m_clueset; }
        }
        public override void Load(System.Xml.Linq.XElement elem)
        {
            base.Load(elem);
            m_clueset.Load(this, elem.Element("clueset"));
        }

        private ClueSet m_clueset;
    }
}