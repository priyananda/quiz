using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace Shenoy.Quiz.Model
{
    public class ObjectWithSlide
    {
        public ObjectWithSlide(int id)
        {
            m_slideid = id;
        }

        public virtual int SlideId { get { return m_slideid; } }
        public AVNode AVData { get { return m_avnode; } }

        protected void LoadVideoData(XElement elem)
        {
            var videoNode = elem.Element("video");
            if (videoNode == null)
                return;
            m_avnode = AVNode.Parse(videoNode);
        }

        private int m_slideid;
        private AVNode m_avnode;
    }
}
