using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Shenoy.Quiz.Model
{
    public class AVNode
    {
        public int X { get { return x; } }
        public int Y { get { return y; } }
        public int W { get { return w; } }
        public int H { get { return h; } }
        public String Source { get { return source; } }
        public Rect Location { get { return new Rect(x, y, w, h); } }

        public static AVNode Parse(XElement elem)
        {
            AVNode node = new AVNode();
            node.x = Int32.Parse(elem.Attribute("x").Value);
            node.y = Int32.Parse(elem.Attribute("y").Value);
            node.w = Int32.Parse(elem.Attribute("w").Value);
            node.h = Int32.Parse(elem.Attribute("h").Value);
            node.source = elem.Attribute("src").Value;
            return node;
        }

        private string source;
        private int x, y, w, h;
    }
}
