using System;
namespace Shenoy.Quiz.Model
{
    public class MetaModifierState
    {
        public static MetaModifierState Current
        {
            get
            {
                if (s_state == null)
                    s_state = new MetaModifierState();
                return s_state;
            }
        }

        public MetaModifierState()
        {
            m_modifiers = new MetaModifier[(int) Celeb._Max];
        }

        public bool IsEnabled(Celeb celeb)
        {
            return m_modifiers[(int)celeb].IsActive;
        }

        public void Activate(Celeb celeb)
        {
            m_modifiers[(int)celeb].Activate();
            if (OnStateChanged != null)
                OnStateChanged(this, celeb);
        }

        public event Action<MetaModifierState, Celeb> OnStateChanged;

        private MetaModifier[] m_modifiers;
        private static MetaModifierState s_state;
    }
}