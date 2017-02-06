using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Shenoy.Quiz.Model
{
    class ClueSet : List<Clue>
    {
        public void Load(Question parent, XElement cselem)
        {
            var clueKids = cselem.Elements("clue");
            foreach (var clue in clueKids)
                this.Add(Clue.Create(parent, clue));
        }
    }
}