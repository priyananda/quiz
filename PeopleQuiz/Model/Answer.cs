using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Shenoy.Quiz.Model
{
    class Answer : ObjectWithSlide
    {
        public static Answer Parse(Question q, XElement answerNode)
        {
            int slideid = Int32.Parse(answerNode.Attribute("slideid").Value);
            Answer answer = new Answer(q, slideid);
            if (answerNode.Attribute("endslide") != null)
            {
                int slideRangeEnd = Int32.Parse(answerNode.Attribute("endslide").Value);
                answer.SetSlideRangeEnd(slideRangeEnd);
            }
            return answer;
        }
        public Answer(Question q, int slideid):
            base(slideid)
        {
            m_question = q;
            m_slideRangeEnd = slideid;
        }

        public void SetSlideRangeEnd(int end)
        {
            m_slideRangeEnd = end;
        }

        public Question Q { get { return m_question; } }
        public int SlideRangeStart { get { return this.SlideId; } }
        public int SlideRangeEnd { get { return m_slideRangeEnd; } }

        private Question m_question;
        private int m_slideRangeEnd;
    }
}