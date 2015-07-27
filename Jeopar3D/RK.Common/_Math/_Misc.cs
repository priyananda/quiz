using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RK.Common
{
    public enum EdgeOrder
    {
        Unknown,

        Clockwise,

        CounterClockwise
    }

    public struct Polygon2DMergeOptions
    {
        public static readonly Polygon2DMergeOptions Default = new Polygon2DMergeOptions();

        public bool MakeMergepointSpaceForTriangulation;
    }
}
