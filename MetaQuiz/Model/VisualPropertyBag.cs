using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Model
{
    public enum VisualProperties
    {
        ShowSecondSet,
        ShowMoustache,
        ShowHand,
        ShowMark,
        Max
    }

    public class VisualPropertyBag
    {
        public bool ShowSecondSet
        {
            get { return m_fShowSecondSet; }
            set { m_fShowSecondSet = value; FireEvent(VisualProperties.ShowSecondSet); }
        }

        public bool ShowMoustache
        {
            get { return m_fShowMoustache; }
            set { m_fShowMoustache = value; FireEvent(VisualProperties.ShowMoustache); }
        }

        public bool ShowHand
        {
            get { return m_fShowHand; }
            set { m_fShowHand = value; FireEvent(VisualProperties.ShowHand); }
        }

        public bool ShowMark
        {
            get { return m_fShowMark; }
            set { m_fShowMark = value; FireEvent(VisualProperties.ShowMark); }
        }

        public event Action<VisualProperties> OnPropertyChange;

        private void FireEvent(VisualProperties vp)
        {
            if (OnPropertyChange != null)
                OnPropertyChange(vp);
        }

        private bool m_fShowSecondSet = false;
        private bool m_fShowMoustache = false;
        private bool m_fShowHand = false;
        private bool m_fShowMark = false;
    }
}
