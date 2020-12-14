using System;
using System.Collections.Generic;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    internal class TestSegmentComparer : IComparer<TestSegment>
    {
        public int Compare(TestSegment x, TestSegment y)
        {
            var angleX = CalculateAngle(x);
            var angleY = CalculateAngle(y);

            return (int)(angleX - angleY);
        }

        private static double CalculateAngle(TestSegment segment)
        {
            var distanceX = segment.PointB.X - segment.PointA.X;
            var distanceY = segment.PointB.Y - segment.PointA.Y;
            var length = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

            return distanceY > 0
                ? Math.Acos(distanceX / length)
                : 2.0 * Math.PI - Math.Acos(distanceX / length);
        }
    }
}