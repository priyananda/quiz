using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnQuiz.Model
{
    class Answer : ObjectWithSlide
    {
        public Answer(Question q, int slideid): base(slideid)
        {
            m_question = q;
        }
        public Question Q { get { return m_question; } }
        private Question m_question;
    }
}