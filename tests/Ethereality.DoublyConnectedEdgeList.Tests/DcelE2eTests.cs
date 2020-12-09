using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    public class DcelE2eTests
    {
        [Fact]
        public void Given_a_rectangle_triangle_Should_return_simple_topology()
        {
            var a = new TestPoint(-1, 0);
            var b = new TestPoint(0, 1);
            var c = new TestPoint(1, 0);
            
            var triangle = new[] {
                new TestSegment(a, b),
                 new TestSegment(b, c),
                  new TestSegment(c, a)};

            var dcel = Dcel.FromShape(triangle);

            dcel.Vertices.Should().HaveCount(3);
            dcel.Vertices.Select(v => v.OriginalPoint).Should().BeEquivalentTo(new[] { a, b, c });
        }
    }
}
