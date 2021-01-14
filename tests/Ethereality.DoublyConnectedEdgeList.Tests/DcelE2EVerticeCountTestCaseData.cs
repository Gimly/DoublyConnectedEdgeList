using System.Collections;
using System.Collections.Generic;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    internal static class TestCaseShapeProvider
    {
        internal static TestSegment[] OneSegment()
        {
            var p1 = new TestPoint(0, 0);
            var p2 = new TestPoint(0, 1);

            var s1 = new TestSegment(p1, p2);

            return new[] { s1 };
        }

        internal static TestSegment[] TwoSegments()
        {
            var p1 = new TestPoint(0, 0);
            var p2 = new TestPoint(10, 1);
            var p3 = new TestPoint(20, 0);

            var s1 = new TestSegment(p1, p2);
            var s2 = new TestSegment(p2, p3);

            return new[] { s1, s2 };
        }

        internal static TestSegment[] RightTriangle()
        {
            var p1 = new TestPoint(-1, 2);
            var p2 = new TestPoint(4, 3);
            var p3 = new TestPoint(5, -2);

            var s1 = new TestSegment(p1, p2);
            var s2 = new TestSegment(p2, p3);
            var s3 = new TestSegment(p3, p1);

            return new[] { s1, s2, s3 };
        }
    }

    internal class DcelE2EVerticesCountTestCaseData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {TestCaseShapeProvider.OneSegment(), 2};
            yield return new object[] {TestCaseShapeProvider.TwoSegments(), 3};
            yield return new object[] { TestCaseShapeProvider.RightTriangle(), 3};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}