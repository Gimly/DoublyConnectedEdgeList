
using System.Drawing;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    internal class TestSegment : ISegment<TestPoint>
    {
        public TestSegment(TestPoint pointA, TestPoint pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }

        public TestPoint PointA { get; }
        public TestPoint PointB { get; }

        public int CompareTo(ISegment<TestPoint> other)
        {
            throw new System.NotImplementedException();
        }
    }
}
