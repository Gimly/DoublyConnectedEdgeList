using System;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    internal class TestPoint : IEquatable<TestPoint>
    {
        public TestPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public bool Equals(TestPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public override string ToString() => $"({X},{Y})";

        public override bool Equals(object obj)
        {
            return Equals(obj as TestPoint);
        }
    }
}
