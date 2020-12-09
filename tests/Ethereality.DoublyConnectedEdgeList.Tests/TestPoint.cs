using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    internal class TestPoint : IComparable<TestPoint>, IEquatable<TestPoint>
    {
        public TestPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public int CompareTo(TestPoint other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(TestPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
