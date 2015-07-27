using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RK.Common.Tests
{
    [TestClass]
    public class PolygonMathematicsTests
    {
        /// <summary>
        /// Tests the line to line intersection.
        /// </summary>
        [TestMethod]
        public void TestLineToLineIntersection()
        {
            //Test case 1
            Line2D lineLeft = new Line2D(0f, 0f, 10f, 0f);
            Line2D lineRight = new Line2D(5f, 5f, 5f, 2f);
            var intersectionResult = lineLeft.Intersect(lineRight);
            Assert.IsFalse(intersectionResult.Item1, "Lines should not intersect here!");

            //Test case 2
            lineLeft = new Line2D(0f, 0f, 10f, 0f);
            lineRight = new Line2D(5f, 5f, 5f, -5f);
            intersectionResult = lineLeft.Intersect(lineRight);
            Assert.IsTrue(intersectionResult.Item1, "Lines should intersect here!");
        }

        [TestMethod]
        public void TestSimpleTriangulateWithHole()
        {
            //Test case 1
            Polygon2D outerPolygon = new Polygon2D(new Vector2[]
            {
                new Vector2(5f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 5f),
                new Vector2(5f, 5f)
            });
            Polygon2D innerPolygon = new Polygon2D(new Vector2[]
            {
                new Vector2(4f, 4f),
                new Vector2(1f, 4f),
                new Vector2(1f, 1f),
                new Vector2(4f, 1f)
            });

            Polygon2D result = outerPolygon.MergeWithHole(innerPolygon, Polygon2DMergeOptions.Default);
            var blub = result.TriangulateUsingCuttingEars();
        }
    }
}
