using System;
using System.Linq;

using FluentAssertions;

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
            var result = dcelFactory.FromShape(new[] {segment});

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
            face.HalfEdges.Should().Contain(new[] {firstHalfEdge, secondHalfEdge});
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
            var result = dcelFactory.FromShape(new[] {segmentA, segmentB});

            // Assert Vertices
            result.Vertices.Should().HaveCount(3);
            result.Vertices.Select(v => v.OriginalPoint).Should().Contain(new[] {a, b, c});

            var vertexA = result.Vertices.Single(v => v.OriginalPoint == a);
            var vertexB = result.Vertices.Single(v => v.OriginalPoint == b);
            var vertexC = result.Vertices.Single(v => v.OriginalPoint == c);

            // Assert HalfEdges
            result.HalfEdges.Should().HaveCount(4);
            var halfEdgeAB = result.FindHalfEdge(a, b);
            var halfEdgeBA = result.FindHalfEdge(b, a);
            var halfEdgeBC = result.FindHalfEdge(b, c);
            var halfEdgeCB = result.FindHalfEdge(c, b);

            result.HalfEdges.Should().Contain(new[] {halfEdgeAB, halfEdgeBC, halfEdgeCB, halfEdgeBA});

            halfEdgeAB.Origin.Should().Be(vertexA);
            halfEdgeBA.Origin.Should().Be(vertexB);
            halfEdgeBC.Origin.Should().Be(vertexB);
            halfEdgeCB.Origin.Should().Be(vertexC);

            // Assert Vertices
            vertexA.HalfEdges.Should().ContainSingle().Which.Should().Be(halfEdgeAB);

            vertexB.HalfEdges.Should().HaveCount(2);
            vertexB.HalfEdges.Should().Contain(new[] {halfEdgeBA, halfEdgeBC});
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
            var face = result.Faces.Should().ContainSingle().Subject;

            face.HalfEdges.Should().HaveCount(4);
            face.HalfEdges.Should().Contain(new[] {halfEdgeAB, halfEdgeBA, halfEdgeBC, halfEdgeCB});

            face.HalfEdges.All(he => he.Face == face).Should().BeTrue();
        }

        [Fact]
        public void Given_two_disjoint_segments_Should_return_valid_dcel()
        {
            // Arrange
            var a = new TestPoint(-10, 5);
            var b = new TestPoint(-2, 3);
            var c = new TestPoint(2, 3);
            var d = new TestPoint(10, 4);

            var segmentA = new TestSegment(a, b);
            var segmentB = new TestSegment(c, d);

            var shape = new[] { segmentA, segmentB };

            var dcelFactory = new DcelFactory<TestSegment, TestPoint>(new TestSegmentComparer());

            // Act
            var actualDcel = dcelFactory.FromShape(shape);

            // Assert Vertices
            actualDcel.Vertices.Should().HaveCount(4);
            actualDcel.Vertices.Select(v => v.OriginalPoint).Should().Contain(new []{a, b, c, d});

            var vertexA = actualDcel.Vertices.Single(v => v.OriginalPoint == a);
            var vertexB = actualDcel.Vertices.Single(v => v.OriginalPoint == b);
            var vertexC = actualDcel.Vertices.Single(v => v.OriginalPoint == c);
            var vertexD = actualDcel.Vertices.Single(v => v.OriginalPoint == d);

            // Assert HalfEdges
            actualDcel.HalfEdges.Should().HaveCount(4);
            var halfEdgeAB = actualDcel.FindHalfEdge(a, b);
            var halfEdgeBA = actualDcel.FindHalfEdge(b, a);
            var halfEdgeCD = actualDcel.FindHalfEdge(c, d);
            var halfEdgeDC = actualDcel.FindHalfEdge(d, c);

            actualDcel.HalfEdges.Should().Contain(new[] {halfEdgeAB, halfEdgeBA, halfEdgeCD, halfEdgeDC});

            halfEdgeAB.Origin.Should().Be(vertexA);
            halfEdgeBA.Origin.Should().Be(vertexB);
            halfEdgeCD.Origin.Should().Be(vertexC);
            halfEdgeDC.Origin.Should().Be(vertexD);

            // Assert Vertices
            foreach (var vertex in actualDcel.Vertices)
            {
                vertex.HalfEdges.Should().HaveCount(1);
            }

            vertexA.HalfEdges.Should().ContainSingle().Which.Should().Be(halfEdgeAB);
            vertexB.HalfEdges.Should().ContainSingle().Which.Should().Be(halfEdgeBA);
            vertexC.HalfEdges.Should().ContainSingle().Which.Should().Be(halfEdgeCD);
            vertexD.HalfEdges.Should().ContainSingle().Which.Should().Be(halfEdgeDC);

            // Assert HalfEdges
            halfEdgeAB.Next.Should().Be(halfEdgeBA);
            halfEdgeAB.Twin.Should().Be(halfEdgeBA);
            halfEdgeAB.Previous.Should().Be(halfEdgeBA);
            halfEdgeAB.OriginalSegment.Should().Be(segmentA);

            halfEdgeCD.Next.Should().Be(halfEdgeDC);
            halfEdgeCD.Twin.Should().Be(halfEdgeDC);
            halfEdgeCD.Previous.Should().Be(halfEdgeDC);
            halfEdgeCD.OriginalSegment.Should().Be(segmentB);

            // Assert Faces
            actualDcel.Faces.Should().HaveCount(2);
            var face1 = actualDcel.Faces[0];
            var face2 = actualDcel.Faces[1];

            face1.HalfEdges.Should().HaveCount(2);
            face1.HalfEdges.All(he => he.Face == face1).Should().BeTrue();
            face1.HalfEdges.Should().Contain(new []{halfEdgeAB, halfEdgeBA});

            face2.HalfEdges.Should().HaveCount(2);
            face2.HalfEdges.All(he => he.Face == face2).Should().BeTrue();
            face2.HalfEdges.Should().Contain(new[] { halfEdgeCD, halfEdgeDC });
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
            var result = dcelFactory.FromShape(triangle);

            // Assert Vertices
            result.Vertices.Should().HaveCount(3);
            result.Vertices.Select(v => v.OriginalPoint).Should().Contain(new[] {a, b, c});

            var vertexA = result.Vertices.Single(v => v.OriginalPoint == a);
            var vertexB = result.Vertices.Single(v => v.OriginalPoint == b);
            var vertexC = result.Vertices.Single(v => v.OriginalPoint == c);

            // Assert HalfEdges
            result.HalfEdges.Should().HaveCount(6);
            var halfEdgeAB = result.FindHalfEdge(a, b);
            var halfEdgeBA = result.FindHalfEdge(b, a);
            var halfEdgeBC = result.FindHalfEdge(b, c);
            var halfEdgeCB = result.FindHalfEdge(c, b);
            var halfEdgeCA = result.FindHalfEdge(c, a);
            var halfEdgeAC = result.FindHalfEdge(a, c);

            result.HalfEdges
                      .Should()
                      .Contain(new[] {halfEdgeAB, halfEdgeBA, halfEdgeBC, halfEdgeCB, halfEdgeCA, halfEdgeAC});

            halfEdgeAB.Origin.Should().Be(vertexA);
            halfEdgeBA.Origin.Should().Be(vertexB);
            halfEdgeBC.Origin.Should().Be(vertexB);
            halfEdgeCB.Origin.Should().Be(vertexC);
            halfEdgeCA.Origin.Should().Be(vertexC);
            halfEdgeAC.Origin.Should().Be(vertexA);

            // Assert Vertices
            foreach (var vertex in result.Vertices)
            {
                vertex.HalfEdges.Should().HaveCount(2);
            }

            vertexA.HalfEdges.Should().Contain(new[] {halfEdgeAB, halfEdgeAC});
            vertexB.HalfEdges.Should().Contain(new[] {halfEdgeBA, halfEdgeBC});
            vertexC.HalfEdges.Should().Contain(new[] {halfEdgeCB, halfEdgeCA});

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
            result.Faces.Should().HaveCount(2);
            var face1 = result.Faces[0];
            var face2 = result.Faces[1];

            face1.HalfEdges.Should().HaveCount(3);
            face1.HalfEdges.All(he => he.Face == face1).Should().BeTrue();
            face1.HalfEdges.Should().Contain(new[] {halfEdgeAB, halfEdgeBC, halfEdgeCA});

            face2.HalfEdges.Should().HaveCount(3);
            face2.HalfEdges.All(he => he.Face == face2).Should().BeTrue();
            face2.HalfEdges.Should().Contain(new[] {halfEdgeAC, halfEdgeCB, halfEdgeBA});
        }

        [Fact]
        public void Given_a_shape_with_hole_Should_return_valid_dcel()
        {
            // Arrange
            var a = new TestPoint(-60, 0);
            var b = new TestPoint(60, 0);
            var c = new TestPoint(40, 40);
            var d = new TestPoint(-40, 40);
            var e = new TestPoint(30, 10);
            var f = new TestPoint(-30, 10);
            var g = new TestPoint(-30, 30);
            var h = new TestPoint(30, 30);

            var segmentA = new TestSegment(a, b);
            var segmentB = new TestSegment(b, c);
            var segmentC = new TestSegment(c, d);
            var segmentD = new TestSegment(d, a);
            var segmentE = new TestSegment(e, f);
            var segmentF = new TestSegment(f, g);
            var segmentG = new TestSegment(g, h);
            var segmentH = new TestSegment(h, e);

            var shape = new[] {segmentA, segmentB, segmentC, segmentD, segmentE, segmentF, segmentG, segmentH};

            var dcelFactory = new DcelFactory<TestSegment, TestPoint>(new TestSegmentComparer());

            // Act
            var result = dcelFactory.FromShape(shape);

            // Assert Vertices
            result.Vertices.Should().HaveCount(8);
            result.Vertices.Select(v => v.OriginalPoint).Should().Contain(new[] {a, b, c, d, e, f, g, h});

            var vertexA = result.Vertices.Single(v => v.OriginalPoint == a);
            var vertexB = result.Vertices.Single(v => v.OriginalPoint == b);
            var vertexC = result.Vertices.Single(v => v.OriginalPoint == c);
            var vertexD = result.Vertices.Single(v => v.OriginalPoint == d);
            var vertexE = result.Vertices.Single(v => v.OriginalPoint == e);
            var vertexF = result.Vertices.Single(v => v.OriginalPoint == f);
            var vertexG = result.Vertices.Single(v => v.OriginalPoint == g);
            var vertexH = result.Vertices.Single(v => v.OriginalPoint == h);

            // Assert HalfEdges
            result.HalfEdges.Should().HaveCount(16);
            var halfEdgeAB = result.FindHalfEdge(a, b);
            var halfEdgeBA = result.FindHalfEdge(b, a);
            var halfEdgeBC = result.FindHalfEdge(b, c);
            var halfEdgeCB = result.FindHalfEdge(c, b);
            var halfEdgeCD = result.FindHalfEdge(c, d);
            var halfEdgeDC = result.FindHalfEdge(d, c);
            var halfEdgeDA = result.FindHalfEdge(d, a);
            var halfEdgeAD = result.FindHalfEdge(a, d);
            var halfEdgeEF = result.FindHalfEdge(e, f);
            var halfEdgeFE = result.FindHalfEdge(f, e);
            var halfEdgeFG = result.FindHalfEdge(f, g);
            var halfEdgeGF = result.FindHalfEdge(g, f);
            var halfEdgeGH = result.FindHalfEdge(g, h);
            var halfEdgeHG = result.FindHalfEdge(h, g);
            var halfEdgeHE = result.FindHalfEdge(h, e);
            var halfEdgeEH = result.FindHalfEdge(e, h);

            result.HalfEdges.Should()
                      .Contain(
                          new[]
                          {
                              halfEdgeAB,
                              halfEdgeBA,
                              halfEdgeBC,
                              halfEdgeCB,
                              halfEdgeCD,
                              halfEdgeDC,
                              halfEdgeDA,
                              halfEdgeAD,
                              halfEdgeEF,
                              halfEdgeFE,
                              halfEdgeFG,
                              halfEdgeGF,
                              halfEdgeGH,
                              halfEdgeHG,
                              halfEdgeHE,
                              halfEdgeEH
                          });

            halfEdgeAB.Origin.Should().Be(vertexA);
            halfEdgeBA.Origin.Should().Be(vertexB);
            halfEdgeBC.Origin.Should().Be(vertexB);
            halfEdgeCB.Origin.Should().Be(vertexC);
            halfEdgeCD.Origin.Should().Be(vertexC);
            halfEdgeDC.Origin.Should().Be(vertexD);
            halfEdgeDA.Origin.Should().Be(vertexD);
            halfEdgeAD.Origin.Should().Be(vertexA);
            halfEdgeEF.Origin.Should().Be(vertexE);
            halfEdgeFE.Origin.Should().Be(vertexF);
            halfEdgeFG.Origin.Should().Be(vertexF);
            halfEdgeGF.Origin.Should().Be(vertexG);
            halfEdgeGH.Origin.Should().Be(vertexG);
            halfEdgeHG.Origin.Should().Be(vertexH);
            halfEdgeHE.Origin.Should().Be(vertexH);
            halfEdgeEH.Origin.Should().Be(vertexE);

            // Assert Vertices
            foreach (var vertex in result.Vertices)
            {
                vertex.HalfEdges.Should().HaveCount(2);
            }

            vertexA.HalfEdges.Should().Contain(new[] {halfEdgeAB, halfEdgeAD});
            vertexB.HalfEdges.Should().Contain(new[] {halfEdgeBA, halfEdgeBC});
            vertexC.HalfEdges.Should().Contain(new[] {halfEdgeCD, halfEdgeCB});
            vertexD.HalfEdges.Should().Contain(new[] {halfEdgeDC, halfEdgeDA});
            vertexE.HalfEdges.Should().Contain(new[] {halfEdgeEF, halfEdgeEH});
            vertexF.HalfEdges.Should().Contain(new[] {halfEdgeFG, halfEdgeFE});
            vertexG.HalfEdges.Should().Contain(new[] {halfEdgeGH, halfEdgeGF});
            vertexH.HalfEdges.Should().Contain(new[] {halfEdgeHG, halfEdgeHE});

            // Assert HalfEdges
            halfEdgeAB.Next.Should().Be(halfEdgeBC);
            halfEdgeAB.Twin.Should().Be(halfEdgeBA);
            halfEdgeAB.Previous.Should().Be(halfEdgeDA);
            halfEdgeAB.OriginalSegment.Should().Be(segmentA);

            halfEdgeBA.Next.Should().Be(halfEdgeAD);
            halfEdgeBA.Twin.Should().Be(halfEdgeAB);
            halfEdgeBA.Previous.Should().Be(halfEdgeCB);
            halfEdgeBA.OriginalSegment.Should().Be(segmentA);

            halfEdgeBC.Next.Should().Be(halfEdgeCD);
            halfEdgeBC.Twin.Should().Be(halfEdgeCB);
            halfEdgeBC.Previous.Should().Be(halfEdgeAB);
            halfEdgeBC.OriginalSegment.Should().Be(segmentB);

            halfEdgeCB.Next.Should().Be(halfEdgeBA);
            halfEdgeCB.Twin.Should().Be(halfEdgeBC);
            halfEdgeCB.Previous.Should().Be(halfEdgeDC);
            halfEdgeCB.OriginalSegment.Should().Be(segmentB);

            halfEdgeDA.Next.Should().Be(halfEdgeAB);
            halfEdgeDA.Twin.Should().Be(halfEdgeAD);
            halfEdgeDA.Previous.Should().Be(halfEdgeCD);
            halfEdgeDA.OriginalSegment.Should().Be(segmentD);

            halfEdgeEF.Next.Should().Be(halfEdgeFG);
            halfEdgeEF.Twin.Should().Be(halfEdgeFE);
            halfEdgeEF.Previous.Should().Be(halfEdgeHE);
            halfEdgeEF.OriginalSegment.Should().Be(segmentE);

            halfEdgeFE.Next.Should().Be(halfEdgeEH);
            halfEdgeFE.Twin.Should().Be(halfEdgeEF);
            halfEdgeFE.Previous.Should().Be(halfEdgeGF);
            halfEdgeFE.OriginalSegment.Should().Be(segmentE);

            halfEdgeFG.Next.Should().Be(halfEdgeGH);
            halfEdgeFG.Twin.Should().Be(halfEdgeGF);
            halfEdgeFG.Previous.Should().Be(halfEdgeEF);
            halfEdgeFG.OriginalSegment.Should().Be(segmentF);

            halfEdgeGF.Next.Should().Be(halfEdgeFE);
            halfEdgeGF.Twin.Should().Be(halfEdgeFG);
            halfEdgeGF.Previous.Should().Be(halfEdgeHG);
            halfEdgeGF.OriginalSegment.Should().Be(segmentF);

            halfEdgeGH.Next.Should().Be(halfEdgeHE);
            halfEdgeGH.Twin.Should().Be(halfEdgeHG);
            halfEdgeGH.Previous.Should().Be(halfEdgeFG);
            halfEdgeGH.OriginalSegment.Should().Be(segmentG);

            halfEdgeHG.Next.Should().Be(halfEdgeGF);
            halfEdgeHG.Twin.Should().Be(halfEdgeGH);
            halfEdgeHG.Previous.Should().Be(halfEdgeEH);
            halfEdgeHG.OriginalSegment.Should().Be(segmentG);

            halfEdgeHE.Next.Should().Be(halfEdgeEF);
            halfEdgeHE.Twin.Should().Be(halfEdgeEH);
            halfEdgeHE.Previous.Should().Be(halfEdgeGH);
            halfEdgeHE.OriginalSegment.Should().Be(segmentH);

            // Assert Faces
            result.Faces.Should().HaveCount(4);
            var face1 = result.Faces[0];
            var face2 = result.Faces[1];
            var face3 = result.Faces[2];
            var face4 = result.Faces[3];

            face1.HalfEdges.Should().HaveCount(4);
            face1.HalfEdges.All(he => he.Face == face1).Should().BeTrue();
            face1.HalfEdges.Should().Contain(new[] {halfEdgeAB, halfEdgeBC, halfEdgeCD, halfEdgeDA});

            face3.HalfEdges.Should().HaveCount(4);
            face3.HalfEdges.All(he => he.Face == face3).Should().BeTrue();
            face3.HalfEdges.Should().Contain(new[] {halfEdgeEF, halfEdgeFG, halfEdgeGH, halfEdgeHE});

            face2.HalfEdges.Should().HaveCount(4);
            face2.HalfEdges.All(he => he.Face == face2).Should().BeTrue();
            face2.HalfEdges.Should().Contain(new[] {halfEdgeBA, halfEdgeAD, halfEdgeDC, halfEdgeCB});

            face4.HalfEdges.Should().HaveCount(4);
            face4.HalfEdges.All(he => he.Face == face4).Should().BeTrue();
            face4.HalfEdges.Should().Contain(new[] {halfEdgeFE, halfEdgeEH, halfEdgeHG, halfEdgeGF});
        }

        [Fact]
        public void Given_a_complex_shape_Should_return_valid_dcel()
        {
            // Arrange
            var a = new TestPoint(-60, 0);
            var b = new TestPoint(60, 0);
            var c = new TestPoint(40, 40);
            var d = new TestPoint(20, 80);
            var e = new TestPoint(0, 120);
            var f = new TestPoint(-20, 80);
            var g = new TestPoint(-40, 40);
            var h = new TestPoint(0, 40);

            var segmentA = new TestSegment(a, b);
            var segmentB = new TestSegment(b, c);
            var segmentC = new TestSegment(c, d);
            var segmentD = new TestSegment(d, e);
            var segmentE = new TestSegment(e, f);
            var segmentF = new TestSegment(f, g);
            var segmentG = new TestSegment(g, h);
            var segmentH = new TestSegment(d, h);
            var segmentI = new TestSegment(h, c);
            var segmentJ = new TestSegment(f, d);
            var segmentK = new TestSegment(h, f);
            var segmentL = new TestSegment(g, a);

            var shape = new[]
            {
                segmentA, segmentB, segmentC, segmentD, segmentE, segmentF,
                segmentG, segmentH, segmentI, segmentJ, segmentK, segmentL
            };

            var dcelFactory = new DcelFactory<TestSegment, TestPoint>(new TestSegmentComparer());

            // Act
            var result = dcelFactory.FromShape(shape);

            // Assert Vertices
            result.Vertices.Should().HaveCount(8);
            result.Vertices.Select(v => v.OriginalPoint).Should().BeEquivalentTo(a, b, c, d, e, f, g, h);

            var vertexA = result.Vertices.Single(v => v.OriginalPoint == a);
            var vertexB = result.Vertices.Single(v => v.OriginalPoint == b);
            var vertexC = result.Vertices.Single(v => v.OriginalPoint == c);
            var vertexD = result.Vertices.Single(v => v.OriginalPoint == d);
            var vertexE = result.Vertices.Single(v => v.OriginalPoint == e);
            var vertexF = result.Vertices.Single(v => v.OriginalPoint == f);
            var vertexG = result.Vertices.Single(v => v.OriginalPoint == g);
            var vertexH = result.Vertices.Single(v => v.OriginalPoint == h);

            // Assert HalfEdges
            result.HalfEdges.Should().HaveCount(24);
            var halfEdgeAB = result.FindHalfEdge(a, b);
            var halfEdgeBA = result.FindHalfEdge(b, a);
            var halfEdgeBC = result.FindHalfEdge(b, c);
            var halfEdgeCB = result.FindHalfEdge(c, b);
            var halfEdgeCD = result.FindHalfEdge(c, d);
            var halfEdgeDC = result.FindHalfEdge(d, c);
            var halfEdgeDE = result.FindHalfEdge(d, e);
            var halfEdgeED = result.FindHalfEdge(e, d);
            var halfEdgeEF = result.FindHalfEdge(e, f);
            var halfEdgeFE = result.FindHalfEdge(f, e);
            var halfEdgeFG = result.FindHalfEdge(f, g);
            var halfEdgeGF = result.FindHalfEdge(g, f);
            var halfEdgeGH = result.FindHalfEdge(g, h);
            var halfEdgeHG = result.FindHalfEdge(h, g);
            var halfEdgeDH = result.FindHalfEdge(d, h);
            var halfEdgeHD = result.FindHalfEdge(h, d);
            var halfEdgeHC = result.FindHalfEdge(h, c);
            var halfEdgeCH = result.FindHalfEdge(c, h);
            var halfEdgeFD = result.FindHalfEdge(f, d);
            var halfEdgeDF = result.FindHalfEdge(d, f);
            var halfEdgeHF = result.FindHalfEdge(h, f);
            var halfEdgeFH = result.FindHalfEdge(f, h);
            var halfEdgeGA = result.FindHalfEdge(g, a);
            var halfEdgeAG = result.FindHalfEdge(a, g);

            result.HalfEdges.Should()
                      .Contain(
                          new[]
                          {
                              halfEdgeAB,
                              halfEdgeBA,
                              halfEdgeBC,
                              halfEdgeCB,
                              halfEdgeCD,
                              halfEdgeDC,
                              halfEdgeDE,
                              halfEdgeED,
                              halfEdgeEF,
                              halfEdgeFE,
                              halfEdgeFG,
                              halfEdgeGF,
                              halfEdgeGH,
                              halfEdgeHG,
                              halfEdgeDH,
                              halfEdgeHD,
                              halfEdgeHC,
                              halfEdgeCH,
                              halfEdgeFD,
                              halfEdgeDF,
                              halfEdgeHF,
                              halfEdgeFH,
                              halfEdgeGA,
                              halfEdgeAG
                          });

            halfEdgeAB.Origin.Should().Be(vertexA);
            halfEdgeBA.Origin.Should().Be(vertexB);
            halfEdgeBC.Origin.Should().Be(vertexB);
            halfEdgeCB.Origin.Should().Be(vertexC);
            halfEdgeCD.Origin.Should().Be(vertexC);
            halfEdgeDC.Origin.Should().Be(vertexD);
            halfEdgeDE.Origin.Should().Be(vertexD);
            halfEdgeED.Origin.Should().Be(vertexE);
            halfEdgeEF.Origin.Should().Be(vertexE);
            halfEdgeFE.Origin.Should().Be(vertexF);
            halfEdgeFG.Origin.Should().Be(vertexF);
            halfEdgeGF.Origin.Should().Be(vertexG);
            halfEdgeGH.Origin.Should().Be(vertexG);
            halfEdgeHG.Origin.Should().Be(vertexH);
            halfEdgeDH.Origin.Should().Be(vertexD);
            halfEdgeHD.Origin.Should().Be(vertexH);
            halfEdgeHC.Origin.Should().Be(vertexH);
            halfEdgeCH.Origin.Should().Be(vertexC);
            halfEdgeFD.Origin.Should().Be(vertexF);
            halfEdgeDF.Origin.Should().Be(vertexD);
            halfEdgeHF.Origin.Should().Be(vertexH);
            halfEdgeFH.Origin.Should().Be(vertexF);
            halfEdgeGA.Origin.Should().Be(vertexG);
            halfEdgeAG.Origin.Should().Be(vertexA);

            // Assert Vertices
            vertexA.HalfEdges.Should().Contain(new[] {halfEdgeAB, halfEdgeAG});
            vertexB.HalfEdges.Should().Contain(new[] {halfEdgeBC, halfEdgeBA});
            vertexC.HalfEdges.Should().Contain(new[] {halfEdgeCB, halfEdgeCH, halfEdgeCD});
            vertexD.HalfEdges.Should().Contain(new[] {halfEdgeDC, halfEdgeDH, halfEdgeDF, halfEdgeDE});
            vertexE.HalfEdges.Should().Contain(new[] {halfEdgeED, halfEdgeEF});
            vertexF.HalfEdges.Should().Contain(new[] {halfEdgeFE, halfEdgeFH, halfEdgeFD, halfEdgeFG});
            vertexG.HalfEdges.Should().Contain(new[] {halfEdgeGF, halfEdgeGH, halfEdgeGA});
            vertexH.HalfEdges.Should().Contain(new[] {halfEdgeHG, halfEdgeHC, halfEdgeHF, halfEdgeHD});

            halfEdgeAB.Next.Should().Be(halfEdgeBC);
            halfEdgeAB.Twin.Should().Be(halfEdgeBA);
            halfEdgeAB.Previous.Should().Be(halfEdgeGA);
            halfEdgeAB.OriginalSegment.Should().Be(segmentA);

            halfEdgeBA.Next.Should().Be(halfEdgeAG);
            halfEdgeBA.Twin.Should().Be(halfEdgeAB);
            halfEdgeBA.Previous.Should().Be(halfEdgeCB);
            halfEdgeBA.OriginalSegment.Should().Be(segmentA);

            halfEdgeBC.Next.Should().Be(halfEdgeCD);
            halfEdgeBC.Twin.Should().Be(halfEdgeCB);
            halfEdgeBC.Previous.Should().Be(halfEdgeAB);
            halfEdgeBC.OriginalSegment.Should().Be(segmentB);

            halfEdgeCB.Next.Should().Be(halfEdgeBA);
            halfEdgeCB.Twin.Should().Be(halfEdgeBC);
            halfEdgeCB.Previous.Should().Be(halfEdgeHC);
            halfEdgeCB.OriginalSegment.Should().Be(segmentB);

            halfEdgeCD.Next.Should().Be(halfEdgeDE);
            halfEdgeCD.Twin.Should().Be(halfEdgeDC);
            halfEdgeCD.Previous.Should().Be(halfEdgeBC);
            halfEdgeCD.OriginalSegment.Should().Be(segmentC);

            halfEdgeDC.Next.Should().Be(halfEdgeCH);
            halfEdgeDC.Twin.Should().Be(halfEdgeCD);
            halfEdgeDC.Previous.Should().Be(halfEdgeHD);
            halfEdgeDC.OriginalSegment.Should().Be(segmentC);

            halfEdgeDE.Next.Should().Be(halfEdgeEF);
            halfEdgeDE.Twin.Should().Be(halfEdgeED);
            halfEdgeDE.Previous.Should().Be(halfEdgeCD);
            halfEdgeDE.OriginalSegment.Should().Be(segmentD);

            halfEdgeED.Next.Should().Be(halfEdgeDF);
            halfEdgeED.Twin.Should().Be(halfEdgeDE);
            halfEdgeED.Previous.Should().Be(halfEdgeFE);
            halfEdgeED.OriginalSegment.Should().Be(segmentD);

            halfEdgeEF.Next.Should().Be(halfEdgeFG);
            halfEdgeEF.Twin.Should().Be(halfEdgeFE);
            halfEdgeEF.Previous.Should().Be(halfEdgeDE);
            halfEdgeEF.OriginalSegment.Should().Be(segmentE);

            halfEdgeFE.Next.Should().Be(halfEdgeED);
            halfEdgeFE.Twin.Should().Be(halfEdgeEF);
            halfEdgeFE.Previous.Should().Be(halfEdgeDF);
            halfEdgeFE.OriginalSegment.Should().Be(segmentE);

            halfEdgeFG.Next.Should().Be(halfEdgeGA);
            halfEdgeFG.Twin.Should().Be(halfEdgeGF);
            halfEdgeFG.Previous.Should().Be(halfEdgeEF);
            halfEdgeFG.OriginalSegment.Should().Be(segmentF);

            halfEdgeGF.Next.Should().Be(halfEdgeFH);
            halfEdgeGF.Twin.Should().Be(halfEdgeFG);
            halfEdgeGF.Previous.Should().Be(halfEdgeHG);
            halfEdgeGF.OriginalSegment.Should().Be(segmentF);

            halfEdgeGH.Next.Should().Be(halfEdgeHC);
            halfEdgeGH.Twin.Should().Be(halfEdgeHG);
            halfEdgeGH.Previous.Should().Be(halfEdgeAG);
            halfEdgeGH.OriginalSegment.Should().Be(segmentG);

            halfEdgeHG.Next.Should().Be(halfEdgeGF);
            halfEdgeHG.Twin.Should().Be(halfEdgeGH);
            halfEdgeHG.Previous.Should().Be(halfEdgeFH);
            halfEdgeHG.OriginalSegment.Should().Be(segmentG);

            halfEdgeDH.Next.Should().Be(halfEdgeHF);
            halfEdgeDH.Twin.Should().Be(halfEdgeHD);
            halfEdgeDH.Previous.Should().Be(halfEdgeFD);
            halfEdgeDH.OriginalSegment.Should().Be(segmentH);

            halfEdgeHD.Next.Should().Be(halfEdgeDC);
            halfEdgeHD.Twin.Should().Be(halfEdgeDH);
            halfEdgeHD.Previous.Should().Be(halfEdgeCH);
            halfEdgeHD.OriginalSegment.Should().Be(segmentH);

            halfEdgeHC.Next.Should().Be(halfEdgeCB);
            halfEdgeHC.Twin.Should().Be(halfEdgeCH);
            halfEdgeHC.Previous.Should().Be(halfEdgeGH);
            halfEdgeHC.OriginalSegment.Should().Be(segmentI);

            halfEdgeCH.Next.Should().Be(halfEdgeHD);
            halfEdgeCH.Twin.Should().Be(halfEdgeHC);
            halfEdgeCH.Previous.Should().Be(halfEdgeDC);
            halfEdgeCH.OriginalSegment.Should().Be(segmentI);

            halfEdgeFD.Next.Should().Be(halfEdgeDH);
            halfEdgeFD.Twin.Should().Be(halfEdgeDF);
            halfEdgeFD.Previous.Should().Be(halfEdgeHF);
            halfEdgeFD.OriginalSegment.Should().Be(segmentJ);

            halfEdgeDF.Next.Should().Be(halfEdgeFE);
            halfEdgeDF.Twin.Should().Be(halfEdgeFD);
            halfEdgeDF.Previous.Should().Be(halfEdgeED);
            halfEdgeDF.OriginalSegment.Should().Be(segmentJ);

            halfEdgeHF.Next.Should().Be(halfEdgeFD);
            halfEdgeHF.Twin.Should().Be(halfEdgeFH);
            halfEdgeHF.Previous.Should().Be(halfEdgeDH);
            halfEdgeHF.OriginalSegment.Should().Be(segmentK);

            halfEdgeFH.Next.Should().Be(halfEdgeHG);
            halfEdgeFH.Twin.Should().Be(halfEdgeHF);
            halfEdgeFH.Previous.Should().Be(halfEdgeGF);
            halfEdgeFH.OriginalSegment.Should().Be(segmentK);

            halfEdgeGA.Next.Should().Be(halfEdgeAB);
            halfEdgeGA.Twin.Should().Be(halfEdgeAG);
            halfEdgeGA.Previous.Should().Be(halfEdgeFG);
            halfEdgeGA.OriginalSegment.Should().Be(segmentL);

            halfEdgeAG.Next.Should().Be(halfEdgeGH);
            halfEdgeAG.Twin.Should().Be(halfEdgeGA);
            halfEdgeAG.Previous.Should().Be(halfEdgeBA);
            halfEdgeAG.OriginalSegment.Should().Be(segmentL);

            // Assert Faces
            result.Faces.Should().HaveCount(6);
            var face1 = result.Faces[0];
            var face2 = result.Faces[1];
            var face3 = result.Faces[2];
            var face4 = result.Faces[3];
            var face5 = result.Faces[4];
            var face6 = result.Faces[5];

            face1.HalfEdges.Should().HaveCount(7);
            face1.HalfEdges.All(he => he.Face == face1).Should().BeTrue();
            face1.HalfEdges.Should()
                 .Contain(new[] {halfEdgeAB, halfEdgeBC, halfEdgeCD, halfEdgeDE, halfEdgeEF, halfEdgeFG, halfEdgeGA});

            face2.HalfEdges.Should().HaveCount(5);
            face2.HalfEdges.All(he => he.Face == face2).Should().BeTrue();
            face2.HalfEdges.Should()
                 .Contain(new[] {halfEdgeAG, halfEdgeGH, halfEdgeHC, halfEdgeCB, halfEdgeBA});

            face3.HalfEdges.Should().HaveCount(3);
            face3.HalfEdges.All(he => he.Face == face3).Should().BeTrue();
            face3.HalfEdges.Should()
                 .Contain(new[] {halfEdgeCH, halfEdgeHD, halfEdgeDC});

            face4.HalfEdges.Should().HaveCount(3);
            face4.HalfEdges.All(he => he.Face == face4).Should().BeTrue();
            face4.HalfEdges.Should()
                 .Contain(new[] {halfEdgeFE, halfEdgeED, halfEdgeDF});

            face6.HalfEdges.Should().HaveCount(3);
            face6.HalfEdges.All(he => he.Face == face6).Should().BeTrue();
            face6.HalfEdges.Should()
                 .Contain(new[] {halfEdgeFD, halfEdgeDH, halfEdgeHF});

            face5.HalfEdges.Should().HaveCount(3);
            face5.HalfEdges.All(he => he.Face == face5).Should().BeTrue();
            face5.HalfEdges.Should()
                 .Contain(new[] {halfEdgeHG, halfEdgeGF, halfEdgeFH});
        }

        [Fact]
        public void Given_points_considered_equal_Should_return_same_vertices()
        {
            // Arrange
            var a = new TestPoint(0, 0);
            var b = new TestPoint(0, 1);
            var veryCloseToB = new TestPoint(0 + 1e-11, 1 + 1e-11);
            var c = new TestPoint(0, 2);

            var segmentA = new TestSegment(a, b);
            var segmentB = new TestSegment(veryCloseToB, c);

            // Act
            var dcelFactory = new DcelFactory<TestSegment, TestPoint>(new TestSegmentComparer());
            var result = dcelFactory.FromShape(new[] { segmentA, segmentB });

            // Assert Vertices
            result.Vertices.Should().HaveCount(3);

            var vertexA = result.FindVertex(a);
            var vertexB = result.FindVertex(b);
            var vertexBPrime = result.FindVertex(veryCloseToB);
            var vertexC = result.FindVertex(c);

            vertexA.Should().NotBeNull();
            vertexA!.OriginalPoint.Should().Be(a);

            vertexBPrime.Should().NotBeNull();
            vertexBPrime.Should().BeSameAs(vertexB);
            vertexBPrime!.OriginalPoint.Should().Be(b);

            vertexC.Should().NotBeNull();
            vertexC!.OriginalPoint.Should().Be(c);

            // Assert HalfEdges
            result.HalfEdges.Should().HaveCount(4);
            var halfEdgeAB = result.FindHalfEdge(a, b);
            var halfEdgeBA = result.FindHalfEdge(b, a);
            var halfEdgeBC = result.FindHalfEdge(b, c);
            var halfEdgeCB = result.FindHalfEdge(c, b);

            result.HalfEdges.Should().Contain(new[] { halfEdgeAB, halfEdgeBC, halfEdgeCB, halfEdgeBA });

            halfEdgeAB.Origin.Should().Be(vertexA);
            halfEdgeBA.Origin.Should().Be(vertexB);
            halfEdgeBC.Origin.Should().Be(vertexB);
            halfEdgeCB.Origin.Should().Be(vertexC);

            // Assert Vertices
            vertexA.HalfEdges.Should().ContainSingle().Which.Should().Be(halfEdgeAB);

            vertexB.HalfEdges.Should().HaveCount(2);
            vertexB.HalfEdges.Should().Contain(new[] { halfEdgeBA, halfEdgeBC });
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
            var face = result.Faces.Should().ContainSingle().Subject;

            face.HalfEdges.Should().HaveCount(4);
            face.HalfEdges.Should().Contain(new[] { halfEdgeAB, halfEdgeBA, halfEdgeBC, halfEdgeCB });

            face.HalfEdges.All(he => he.Face == face).Should().BeTrue();
        }
    }
}
