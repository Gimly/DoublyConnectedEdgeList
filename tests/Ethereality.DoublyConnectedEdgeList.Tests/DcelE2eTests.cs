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
            var action = new Action(() => _ = new Vertex<TestSegment, TestPoint>(null));
            action.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("point");
        }

        [Fact]
        public void Given_a_single_segment_Should_return_valid_dcel()
        {
            var a = new TestPoint(0, 0);
            var b = new TestPoint(0, 1);

            var segment = new TestSegment(a, b);

            var dcelFactory = new DcelFactory<TestSegment, TestPoint>(new TestSegmentComparer());
            var result = dcelFactory.FromShape(new[] { segment });

            result.Vertices.Should().HaveCount(2);

            var firstVertex = result.Vertices[0];
            var secondVertex = result.Vertices[1];

            firstVertex.OriginalPoint.Should().Be(a);
            secondVertex.OriginalPoint.Should().Be(b);

            result.HalfEdges.Should().HaveCount(2);

            var firstHalfEdge = result.HalfEdges[0];
            var secondHalfEdge = result.HalfEdges[1];

            firstVertex.HalfEdges.Should().ContainSingle().Which.Should().Be(firstHalfEdge);
            secondVertex.HalfEdges.Should().ContainSingle().Which.Should().Be(secondHalfEdge);

            firstHalfEdge.Origin.Should().Be(firstVertex);
            secondHalfEdge.Origin.Should().Be(secondVertex);

            firstHalfEdge.OriginalSegment.Should().Be(segment);
            secondHalfEdge.OriginalSegment.Should().Be(segment);

            firstHalfEdge.Twin.Should().Be(secondHalfEdge);
            secondHalfEdge.Twin.Should().Be(firstHalfEdge);

            firstHalfEdge.Next.Should().Be(secondHalfEdge);
            firstHalfEdge.Previous.Should().Be(secondHalfEdge);

            secondHalfEdge.Next.Should().Be(firstHalfEdge);
            secondHalfEdge.Previous.Should().Be(firstHalfEdge);

            result.Faces.Should().HaveCount(1);

            var face = result.Faces.Single();

            firstHalfEdge.Face.Should().Be(face);
            secondHalfEdge.Face.Should().Be(face);

            face.HalfEdges.Should().HaveCount(2);
            face.HalfEdges.Should().Contain(new[] { firstHalfEdge, secondHalfEdge });
        }

        [Fact]
        public void Given_a_rectangle_triangle_Should_return_simple_topology()
        {
            var a = new TestPoint(-1, 2);
            var b = new TestPoint(4, 3);
            var c = new TestPoint(5, -2);

            var triangle = new[]
            {
                new TestSegment(a, b),
                new TestSegment(b, c),
                new TestSegment(c, a)
            };

            var dcelFactory = new DcelFactory<TestSegment, TestPoint>(new TestSegmentComparer());
            var dcel = dcelFactory.FromShape(triangle);

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

        //[Fact]
        //public void Given_same_input_as_python_implementation_Should_return_same_output()
        //{
        //    var points =
        //        new[]
        //        {
        //            new TestPoint(0, 5),
        //            new TestPoint(2, 5),
        //            new TestPoint(3, 0),
        //            new TestPoint(0, 0)
        //        };

        //    var shape =
        //        new[]
        //        {
        //            new TestSegment(points[0], points[1]),
        //            new TestSegment(points[1], points[2]),
        //            new TestSegment(points[2], points[3]),
        //            new TestSegment(points[3], points[0]),
        //            new TestSegment(points[0], points[2])
        //        };

        //    var dcelFactory = new DcelFactory<TestSegment, TestPoint>(new TestSegmentComparer());
        //    var dcel = dcelFactory.FromShape(shape);
        //    dcel.Vertices.Should().HaveCount(4);

        //    var firstHalfEdge = dcel.FindHalfEdge(points[2], points[0]);
        //    firstHalfEdge.Should().NotBeNull();

        //    var secondHalfEdge = firstHalfEdge.Next;
        //    secondHalfEdge.OriginalSegment.PointA.Should().Be(points[0]);
        //    secondHalfEdge.OriginalSegment.PointB.Should().Be(points[3]);

        //    var thirdHalfEdge = secondHalfEdge.Next;
        //    thirdHalfEdge.OriginalSegment.PointA.Should().Be(points[3]);
        //    thirdHalfEdge.OriginalSegment.PointB.Should().Be(points[0]);
        //}
    }
}
