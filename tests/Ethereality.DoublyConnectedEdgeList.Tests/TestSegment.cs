namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    internal class TestSegment : IEdge<TestPoint>
    {
        public TestSegment(TestPoint pointA, TestPoint pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }

        public TestPoint PointA { get; }

        public TestPoint PointB { get; }

        public override string ToString() => $"({PointA})->({PointB})";
    }
}
