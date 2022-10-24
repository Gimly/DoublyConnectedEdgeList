using System;
using System.Collections.Generic;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    internal class TestSegmentComparer : IComparer<TestSegment>
    {
        public int Compare(TestSegment x, TestSegment y)
        {
            var reverseX = false;
            var reverseY = false;

            if (x.PointB.Equals(y.PointB))
            {
                reverseX = true;
                reverseY = true;
            }

            if (x.PointA.Equals(y.PointB))
            {
                reverseX = false;
                reverseY = true;
            }

            if (x.PointB.Equals(y.PointA))
            {
                reverseX = true;
                reverseY = false;
            }

            var angleX = CalculateAngle(x, reverseX);
            var angleY = CalculateAngle(y, reverseY);

            return Math.Sign(angleY - angleX);
        }

        private static double CalculateAngle(TestSegment segment, bool reverseDirection)
        {
            var distanceX = segment.PointB.X - segment.PointA.X;
            var distanceY = segment.PointB.Y - segment.PointA.Y;

            return reverseDirection
                ? Math.Atan2(-distanceY, -distanceX)
                : Math.Atan2(distanceY, distanceX);
        }
    }
}
