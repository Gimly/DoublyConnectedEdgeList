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
            // Arrange
            Action action = () => _ = new Vertex<TestSegment, TestPoint>(null);

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("point");
        }

        [Fact]
        public void Given_a_single_segment_Should_return_valid_dcel()
        {
            // Arrange
            var a = new TestPoint(0, 0);
            var b = new TestPoint(0, 1);

            var segment = new TestSegment(a, b);

            // Act
            var dcelFactory = new DcelFactory<TestSegment, TestPoint>(new TestSegmentComparer());
            var result = dcelFactory.FromShape(new[] { segment });

            // Assert Vertices
            result.Vertices.Should().HaveCount(2);

            var firstVertex = result.Vertices[0];
            var secondVertex = result.Vertices[1];

            firstVertex.OriginalPoint.Should().Be(a);
            secondVertex.OriginalPoint.Should().Be(b);

            // Assert HalfEdges
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

            // Assert Faces
            result.Faces.Should().HaveCount(1);

            var face = result.Faces.Single();

            firstHalfEdge.Face.Should().Be(face);
            secondHalfEdge.Face.Should().Be(face);

            face.HalfEdges.Should().HaveCount(2);
            face.HalfEdges.Should().Contain(new[] { firstHalfEdge, secondHalfEdge });
        }

        [Fact]
        public void Given_two_segments_Should_return_valid_dcel()
        {
            // Arrange
            var a = new TestPoint(0, 0);
            var b = new TestPoint(0, 1);
            var c = new TestPoint(0, 2);

            var segmentA = new TestSegment(a, b);
            var segmentB = new TestSegment(b, c);

            // Act
            var dcelFactory = new DcelFactory<TestSegment, TestPoint>(new TestSegmentComparer());
            var actualDcel = dcelFactory.FromShape(new[] { segmentA, segmentB });

            // Assert Vertices
            actualDcel.Vertices.Should().HaveCount(3);
            actualDcel.Vertices.Select(v => v.OriginalPoint).Should().BeEquivalentTo(a, b, c);

            var vertexA = actualDcel.Vertices.Single(v => v.OriginalPoint == a);
            var vertexB = actualDcel.Vertices.Single(v => v.OriginalPoint == b);
            var vertexC = actualDcel.Vertices.Single(v => v.OriginalPoint == c);

            // Assert HalfEdges
            actualDcel.HalfEdges.Should().HaveCount(4);
            var halfEdgeAB = actualDcel.FindHalfEdge(a, b);
            var halfEdgeBA = actualDcel.FindHalfEdge(b, a);
            var halfEdgeBC = actualDcel.FindHalfEdge(b, c);
            var halfEdgeCB = actualDcel.FindHalfEdge(c, b);
            actualDcel.HalfEdges.Should().BeEquivalentTo(halfEdgeAB, halfEdgeBC, halfEdgeCB, halfEdgeBA);

            halfEdgeAB.Origin.Should().Be(vertexA);
            halfEdgeBA.Origin.Should().Be(vertexB);
            halfEdgeBC.Origin.Should().Be(vertexB);
            halfEdgeCB.Origin.Should().Be(vertexC);

            // Assert Vertices
            vertexA.HalfEdges.Should().ContainSingle().Which.Should().Be(halfEdgeAB);

            vertexB.HalfEdges.Should().HaveCount(2);
            vertexB.HalfEdges.Should().BeEquivalentTo(halfEdgeBA, halfEdgeBC);
            vertexC.HalfEdges.Should().ContainSingle().Which.Should().Be(halfEdgeCB);

            // Assert HalfEdges
            halfEdgeAB.Next.Should().Be(halfEdgeBC);
            halfEdgeAB.Twin.Should().Be(halfEdgeBA);
            halfEdgeAB.Previous.Should().Be(halfEdgeBA);
            halfEdgeAB.OriginalSegment.Should().Be(segmentA);

            halfEdgeBC.Next.Should().Be(halfEdgeCB);
            halfEdgeBC.Twin.Should().Be(halfEdgeCB);
            halfEdgeBC.Previous.Should().Be(halfEdgeAB);
            halfEdgeBC.OriginalSegment.Should().Be(segmentB);

            halfEdgeCB.Next.Should().Be(halfEdgeBA);
            halfEdgeCB.Twin.Should().Be(halfEdgeBC);
            halfEdgeCB.Previous.Should().Be(halfEdgeBC);
            halfEdgeCB.OriginalSegment.Should().Be(segmentB);

            halfEdgeBA.Next.Should().Be(halfEdgeAB);
            halfEdgeBA.Twin.Should().Be(halfEdgeAB);
            halfEdgeBA.Previous.Should().Be(halfEdgeCB);
            halfEdgeBA.OriginalSegment.Should().Be(segmentA);

            // Assert Faces
            var face = actualDcel.Faces.Should().ContainSingle().Subject;

            face.HalfEdges.Should().HaveCount(4);
            face.HalfEdges.Should().Contain(new []{halfEdgeAB, halfEdgeBA, halfEdgeBC, halfEdgeCB});

            face.HalfEdges.All(he => he.Face == face).Should().BeTrue();
        }

        [Fact]
        public void Given_a_right_triangle_Should_return_valid_dcel()
        {
            // Arrange
            var a = new TestPoint(-1, 2);
            var b = new TestPoint(4, 3);
            var c = new TestPoint(5, -2);

            var segmentA = new TestSegment(a, b);
            var segmentB = new TestSegment(b, c);
            var segmentC = new TestSegment(c, a);

            var triangle = new[] {segmentA, segmentB, segmentC};

            var dcelFactory = new DcelFactory<TestSegment, TestPoint>(new TestSegmentComparer());

            // Act
            var actualDcel = dcelFactory.FromShape(triangle);

            // Assert Vertices
            actualDcel.Vertices.Should().HaveCount(3);
            actualDcel.Vertices.Select(v => v.OriginalPoint).Should().BeEquivalentTo(a, b, c);

            var vertexA = actualDcel.Vertices.Single(v => v.OriginalPoint == a);
            var vertexB = actualDcel.Vertices.Single(v => v.OriginalPoint == b);
            var vertexC = actualDcel.Vertices.Single(v => v.OriginalPoint == c);

            // Assert HalfEdges
            actualDcel.HalfEdges.Should().HaveCount(6);
            var halfEdgeAB = actualDcel.FindHalfEdge(a, b);
            var halfEdgeBA = actualDcel.FindHalfEdge(b, a);
            var halfEdgeBC = actualDcel.FindHalfEdge(b, c);
            var halfEdgeCB = actualDcel.FindHalfEdge(c, b);
            var halfEdgeCA = actualDcel.FindHalfEdge(c, a);
            var halfEdgeAC = actualDcel.FindHalfEdge(a, c);

            actualDcel.HalfEdges.Should().BeEquivalentTo(halfEdgeAB, halfEdgeBA, halfEdgeBC, halfEdgeCB, halfEdgeCA, halfEdgeAC);

            halfEdgeAB.Origin.Should().Be(vertexA);
            halfEdgeBA.Origin.Should().Be(vertexB);
            halfEdgeBC.Origin.Should().Be(vertexB);
            halfEdgeCB.Origin.Should().Be(vertexC);
            halfEdgeCA.Origin.Should().Be(vertexC);
            halfEdgeAC.Origin.Should().Be(vertexA);

            // Assert Vertices
            foreach (var vertex in actualDcel.Vertices)
            {
                vertex.HalfEdges.Should().HaveCount(2);
            }

            vertexA.HalfEdges.Should().ContainInOrder(halfEdgeAB, halfEdgeAC);
            vertexB.HalfEdges.Should().ContainInOrder(halfEdgeBA, halfEdgeBC);
            vertexC.HalfEdges.Should().ContainInOrder(halfEdgeCB, halfEdgeCA);

            // Assert HalfEdges
            halfEdgeAB.Next.Should().Be(halfEdgeBC);
            halfEdgeAB.Twin.Should().Be(halfEdgeBA);
            halfEdgeAB.Previous.Should().Be(halfEdgeCA);
            halfEdgeAB.OriginalSegment.Should().Be(segmentA);

            halfEdgeBC.Next.Should().Be(halfEdgeCA);
            halfEdgeBC.Twin.Should().Be(halfEdgeCB);
            halfEdgeBC.Previous.Should().Be(halfEdgeAB);
            halfEdgeBC.OriginalSegment.Should().Be(segmentB);

            halfEdgeCA.Next.Should().Be(halfEdgeAB);
            halfEdgeCA.Twin.Should().Be(halfEdgeAC);
            halfEdgeCA.Previous.Should().Be(halfEdgeBC);
            halfEdgeCA.OriginalSegment.Should().Be(segmentC);

            // Assert Faces
            actualDcel.Faces.Should().HaveCount(2);
            var face1 = actualDcel.Faces[0];
            var face2 = actualDcel.Faces[1];

            face1.HalfEdges.Should().HaveCount(3);
            face1.HalfEdges.All(he => he.Face == face1).Should().BeTrue();
            face1.HalfEdges.Should().Contain(new []{halfEdgeAB, halfEdgeBC, halfEdgeCA});

            face2.HalfEdges.Should().HaveCount(3);
            face2.HalfEdges.All(he => he.Face == face2).Should().BeTrue();
            face2.HalfEdges.Should().Contain(new []{halfEdgeAC, halfEdgeCB, halfEdgeBA});
        }

        [Fact]
        public void Given_a_complex_shape_Should_return_valid_dcel()
        {

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
