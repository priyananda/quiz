using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Shenoy.Question.Model
{
    public class QuestionFactory
    {
        public static Question Parse(XElement elem)
        {
            Question ret = null;
            int id = Int32.Parse(elem.Attribute("id").Value);
            switch (elem.Attribute("type").Value)
            {
                case "simple":
                    ret = new SimpleQuestion(id);
                    break;
                case "connect":
                    ret = new ConnectQuestion(id);
                    break;
                case "stagedconnect":
                    ret = new StagedConnectQuestion(id);
                    break;
                case "longlist":
                    ret = new LongListQuestion(id);
                    break;
                case "concept":
                    ret = new Concept(id);
                    break;
            }
            ret.Load(elem);
            return ret;
        }
    }
}
