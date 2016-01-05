using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Model
{
    public class Quiz
    {
        public Quiz()
        {
            s_quiz = this;
        }

        public void Start()
        {
            m_questions = new Questions();
            m_questions.Load("qdata.xml");
            m_metaManager = new MetaModifierManager(this);
            m_vpBag = new VisualPropertyBag();
        }

        public Questions Questions
        {
            get { return m_questions; }
        }

        public MetaModifierManager MetaManager
        {
            get { return m_metaManager; }
        }

        public VisualPropertyBag VProps
        {
            get { return m_vpBag; }
        }

        public static Quiz Current
        {
            get { return s_quiz; }
        }

        private Questions m_questions;
        private MetaModifierManager m_metaManager;
        private VisualPropertyBag m_vpBag;

        private static Quiz s_quiz;
    }
}
