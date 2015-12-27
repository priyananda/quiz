using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Shenoy.Quiz.Model
{
    class SimpleQuestion : Question
    {
        public SimpleQuestion(int id) : base(id, QuestionType.Simple)
        {
        }

        public override void Advance()
        {
            if (!this.IsAnswered)
                AnswerQuestion();
        }

        public override void Load(XElement elem)
        {
            base.Load(elem);
            m_clue = Clue.Create(this, elem.Element("clue"));
        }

        public Clue Clue
        {
            get { return m_clue; }
        }

        Clue m_clue;
    }
}