using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Model
{
    abstract class MetaModifier
    {
        public virtual void Activate()
        {
            m_fActivated = true;
        }

        public virtual bool IsActive
        {
            get
            {
                return m_fActivated;
            }
        }

        public abstract void DoApply();

        protected bool m_fActivated = false;
    }

    class LimitedMetaModifier : MetaModifier
    {
        LimitedMetaModifier(int limit)
        {
            m_limit = limit;
        }

        public override void Activate()
        {
            base.Activate();
            m_counter = m_limit;
        }

        public override bool IsActive
        {
            get
            {
                return base.IsActive && m_counter > 0;
            }
        }

        public override void DoApply()
        {
            if (m_counter > 0)
                m_counter--;
        }

        private int m_limit;
        private int m_counter = 0;
    }
}
