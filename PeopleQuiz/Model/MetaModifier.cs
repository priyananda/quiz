using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Model
{
    public enum MetaModifierState
    {
        Dormant,
        Active,
        Finished
    }

    abstract class MetaModifier
    {
        public virtual void Activate()
        {
            if (m_state == MetaModifierState.Dormant)
                m_state = MetaModifierState.Active;
        }

        public MetaModifierState State
        {
            get { return m_state; }
        }

        public VisualPropertyBag VProps
        {
            get { return Quiz.Current.VProps; }
        }

        public abstract void Apply();

        protected MetaModifierState m_state = MetaModifierState.Dormant;
    }

    class TimeLimitedMetaModifier : MetaModifier
    {
        public TimeLimitedMetaModifier(int limit)
        {
            m_limit = limit;
        }

        public override void Activate()
        {
            base.Activate();
            m_counter = m_limit;
        }

        public override void Apply()
        {
            if (m_counter > 0)
                m_counter--;
            else if (m_counter == 0)
                m_state = MetaModifierState.Finished;
        }

        protected int m_limit;
        protected int m_counter = 0;
    }

    class SingleShotMetaModifier : TimeLimitedMetaModifier
    {
        public SingleShotMetaModifier() : base(1)
        {
           
        }
    }

    class ReusableMetaModifier : MetaModifier
    {
        public override void Apply()
        {
            m_state = MetaModifierState.Dormant;
        }
    }
}
