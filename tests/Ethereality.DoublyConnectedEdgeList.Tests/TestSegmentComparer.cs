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

            return Math.Sign(angleY - angleX);
        }

        private static double CalculateAngle(TestSegment segment)
        {
            var distanceX = segment.PointB.X - segment.PointA.X;
            var distanceY = segment.PointB.Y - segment.PointA.Y;

            return Math.Atan2(distanceY, distanceX);
        }
    }
}