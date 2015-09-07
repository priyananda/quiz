using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shenoy.Question.Model
{
    public class Answer : ObjectWithSlide
    {
        public Answer(Question q, int slideid): base(slideid)
        {
            m_question = q;
        }
        public Question Parent
        {
            get
            {
                return m_question;
            }
        }

        private Question m_question;
    }
}