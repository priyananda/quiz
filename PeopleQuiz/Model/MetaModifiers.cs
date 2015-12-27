using System;
namespace Shenoy.Quiz.Model
{
    public enum MetaModifiers
    {
        RedYellowGreen,
        Pounce,
        MandatoryPounce,
        FrenchBeard,
        RomanTheme,
        DoubleOrNegative,
        DoublesDuration,
        Max
    }

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
            m_isEnabled = new bool[(int) MetaModifiers.Max];
        }

        public bool IsEnabled(MetaModifiers mm)
        {
            return m_isEnabled[(int)mm];
        }

        public void SetEnabled(MetaModifiers mm)
        {
            m_isEnabled[(int)mm] = true;
            if (OnStateChanged != null)
                OnStateChanged(this, mm);
        }

        public event Action<MetaModifierState, MetaModifiers> OnStateChanged;

        private bool[] m_isEnabled;
        private static MetaModifierState s_state;
    }
}