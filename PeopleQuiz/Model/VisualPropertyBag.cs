using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Model
{
    public enum VisualProperties
    {
        ShowFirstSet,
        ShowSecondSet,
        ShowThirdSet,
        EvenOddState,
        ShowJayalalitha,
        ShowOnlyWomen,
        ShowWhitifiedProfile,
        ShowTrafficLights
    }

    public enum KejriwalState
    {
        ShowOnlyEven,
        ShowOnlyOdd,
        ShowAll,
    }

    public class VisualPropertyBag
    {
        public bool ShowFirstSet
        {
            get { return m_fShowFirstSet; }
            set { m_fShowFirstSet = value; FireEvent(VisualProperties.ShowFirstSet); }
        }

        public bool ShowSecondSet
        {
            get { return m_fShowSecondSet; }
            set { m_fShowSecondSet = value; FireEvent(VisualProperties.ShowSecondSet); }
        }

        public bool ShowThirdSet
        {
            get { return m_fShowThirdSet; }
            set { m_fShowThirdSet = value; FireEvent(VisualProperties.ShowThirdSet); }
        }

        public KejriwalState EvenOddState
        {
            get { return m_fEvenOddState; }
            set { m_fEvenOddState = value; FireEvent(VisualProperties.EvenOddState); }
        }

        public bool ShowJayalalitha
        {
            get { return m_fShowjaya; }
            set { m_fShowjaya = value; FireEvent(VisualProperties.ShowJayalalitha); }
        }

        public bool ShowOnlyWomen
        {
            get { return m_fShowOnlyWomen; }
            set { m_fShowOnlyWomen = value; FireEvent(VisualProperties.ShowOnlyWomen); }
        }

        public bool ShowWhitifiedProfile
        {
            get { return m_fShowWhitifiedProfile; }
            set { m_fShowWhitifiedProfile = value; FireEvent(VisualProperties.ShowWhitifiedProfile); }
        }

        public bool ShowTrafficLights
        {
            get { return m_fShowTrafficLights; }
            set { m_fShowTrafficLights = value; FireEvent(VisualProperties.ShowTrafficLights); }
        }

        public event Action<VisualProperties> OnPropertyChange;

        private void FireEvent(VisualProperties vp)
        {
            if (OnPropertyChange != null)
                OnPropertyChange(vp);
        }

        private bool m_fShowFirstSet = false;
        private bool m_fShowSecondSet = false;
        private bool m_fShowThirdSet = false;
        private KejriwalState m_fEvenOddState = KejriwalState.ShowAll;
        private bool m_fShowjaya = false;
        private bool m_fShowOnlyWomen = false;
        private bool m_fShowWhitifiedProfile = false;
        private bool m_fShowTrafficLights = false;
    }
}
