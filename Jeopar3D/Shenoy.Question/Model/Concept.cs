using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Shenoy.Question.Model
{
    public class Concept : Question
    {
        public Concept(int id)
            : base(id, QuestionType.Concept)
        {
        }
        public override void Advance()
        {
        }
        public IEnumerable<int> RelatedQuestions
        {
            get { return m_relatedQuestions; }
        }

        public override void Load(System.Xml.Linq.XElement elem)
        {
            base.Load(elem);
            var children = elem.Elements("related");
            m_relatedQuestions = new int[children.Count()];
            int i = 0;
            foreach (var kid in children)
                m_relatedQuestions[i++] = Int32.Parse((kid as XElement).Attribute("id").Value);
        }

        public override int Points
        {
            get { return 0; }
        }

        private int[] m_relatedQuestions;
    }
}
