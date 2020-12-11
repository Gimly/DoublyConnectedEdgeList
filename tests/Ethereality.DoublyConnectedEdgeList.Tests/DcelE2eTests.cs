using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    public class DcelE2eTests
    {
        [Fact]
        public void When_calling_constructor_Given_null_vertices_Should_throw_ArgumentNullException()
        {
            Action action = new Action(() => new Vertex<TestSegment, TestPoint>(null));
            action.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("point");
        }

        [Fact]
        public void Given_a_rectangle_triangle_Should_return_simple_topology()
        {
            var a = new TestPoint(-1, 2);
            var b = new TestPoint(4, 3);
            var c = new TestPoint(5, -2);

            var triangle = new[] {
                new TestSegment(a, b),
                 new TestSegment(b, c),
                  new TestSegment(c, a)};

            var dcel = Dcel.FromShape<TestSegment, TestPoint>(triangle, new TestSegmentComparer());

            dcel.Vertices.Should().HaveCount(3);
            dcel.Vertices.Select(v => v.OriginalPoint).Should().BeEquivalentTo(new[] { a, b, c });
            foreach (var vertex in dcel.Vertices)
            {
                vertex.HalfEdges.Should().HaveCount(2);
            }

            dcel.HalfEdges.Should().HaveCount(6);
            dcel.HalfEdges.All(halfEdge => halfEdge.Origin is not null).Should().BeTrue();
            dcel.HalfEdges.All(halfEdge => halfEdge.Twin is not null).Should().BeTrue();
            dcel.HalfEdges.All(halfEdge => halfEdge.Previous is not null && halfEdge.Previous is not null).Should().BeTrue();

            dcel.Faces.Should().HaveCount(2);
        }

        
    }
}
