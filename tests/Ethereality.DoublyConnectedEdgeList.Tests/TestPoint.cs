using System;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    internal class TestPoint : IEquatable<TestPoint>
    {
        private const double ComparisonTolerance = 1e-10;

        public TestPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public bool Equals(TestPoint other)
        {
            if (other is null)
            {
                return false;
            }

            return Math.Abs(X - other.X) < ComparisonTolerance
                && Math.Abs(Y - other.Y) < ComparisonTolerance;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public override string ToString() => $"({X},{Y})";

        public override bool Equals(object obj) => Equals(obj as TestPoint);
    }
}
