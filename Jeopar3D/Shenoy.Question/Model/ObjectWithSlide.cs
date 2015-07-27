using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace Shenoy.Question.Model
{
    public class ObjectWithSlide
    {
        public ObjectWithSlide(int id)
        {
            m_slideid = id;
        }

        public virtual int SlideId { get { return m_slideid; } }

        public string VideoUrl
        {
            get { return m_videoUrl; }
            set { m_videoUrl = value; }
        }

        public Rect VideoLocation
        {
            get { return m_loc; }
        }

        protected void LoadVideoData(XElement elem)
        {
            var videoNode = elem.Element("video");
            if (videoNode == null)
                return;
            m_videoUrl = videoNode.Attribute("src").Value;
            int x = Int32.Parse(videoNode.Attribute("x").Value);
            int y = Int32.Parse(videoNode.Attribute("y").Value);
            int w = Int32.Parse(videoNode.Attribute("w").Value);
            int h = Int32.Parse(videoNode.Attribute("h").Value);
            SetBounds(x, y, w, h);
        }

        public void SetBounds(int x, int y, int w, int h)
        {
            m_loc = new Rect(x, y, w, h);
        }

        private int m_slideid;
        private string m_videoUrl;
        private Rect m_loc;
    }
}
