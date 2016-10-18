using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Backend
{
    public class TeamInfo
    {
        public long TeamId { get; set; }
        public String FirstPersonName { get; set; }
        public String SecondPersonName { get; set; }
        public bool IsFinalist { get; set; }

        public long Score { get; set; }

        public void SerializeTo(Dictionary<string, object> propertyBag)
        {
            propertyBag.Add("TeamId", this.TeamId);
            propertyBag.Add("FirstPersonName", this.FirstPersonName);
            propertyBag.Add("SecondPersonName", this.SecondPersonName);
            propertyBag.Add("IsFinalist", this.IsFinalist);
            propertyBag.Add("Score", this.IsFinalist);
        }

        public void Deserialize(Dictionary<string, object> propertyBag)
        {
            this.TeamId = (long)(double)propertyBag["teamid"];
            this.FirstPersonName = (String)propertyBag["firstpersonname"];
            this.SecondPersonName = (String)propertyBag["secondpersonname"];
            this.IsFinalist = (bool)propertyBag["isfinalist"];
            this.Score = (long)(double)propertyBag["score"];
        }
    }
}
