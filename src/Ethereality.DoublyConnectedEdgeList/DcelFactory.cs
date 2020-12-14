using System;
using System.Collections.Generic;
using System.Linq;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class DcelFactory<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        private readonly IComparer<TEdge> _coincidentEdgeComparer;

        public DcelFactory(IComparer<TEdge> coincidentEdgeComparer) =>
            _coincidentEdgeComparer =
                coincidentEdgeComparer ?? throw new ArgumentNullException(nameof(coincidentEdgeComparer));

        public Dcel<TEdge, TPoint> FromShape(IEnumerable<TEdge> edges)
        {
            var verticesDictionary = CreateVertices(edges);

            var halfEdges = CreateHalfEdges(edges, verticesDictionary);
            List<Face<TEdge, TPoint>> faces = CreateFaces(halfEdges);

            return new Dcel<TEdge, TPoint>(
                verticesDictionary.Values.Cast<IVertex<TEdge, TPoint>>(),
                halfEdges.Cast<IHalfEdge<TEdge, TPoint>>(),
                faces.Cast<IFace<TEdge, TPoint>>());
        }

        private static Dictionary<TPoint, Vertex<TEdge, TPoint>> CreateVertices(IEnumerable<TEdge> edges) =>
            edges.Select(s => s.PointA)
                 .Concat(edges.Select(s => s.PointB))
                 .Distinct()
                 .Select(point => new Vertex<TEdge, TPoint>(point))
                 .ToDictionary(value => value.OriginalPoint, value => value);

        private List<HalfEdge<TEdge, TPoint>> CreateHalfEdges(
            IEnumerable<TEdge> edges,
            Dictionary<TPoint, Vertex<TEdge, TPoint>> verticesDictionary)
        {
            var halfEdges = new List<HalfEdge<TEdge, TPoint>>();

            foreach (var segment in edges)
            {
                var pointAVertex = verticesDictionary[segment.PointA];
                var firstHalfEdge = new HalfEdge<TEdge, TPoint>(segment, pointAVertex);
                pointAVertex.HalfEdges.Add(firstHalfEdge);

                var pointBVertex = verticesDictionary[segment.PointB];
                var secondHalfEdge = new HalfEdge<TEdge, TPoint>(segment, pointBVertex)
                {
                    Twin = firstHalfEdge
                };

                pointBVertex.HalfEdges.Add(secondHalfEdge);

                firstHalfEdge.Twin = secondHalfEdge;

                halfEdges.Add(firstHalfEdge);
                halfEdges.Add(secondHalfEdge);
            }

            SetHalfEdgesValues(verticesDictionary);

            return halfEdges;
        }

        private void SetHalfEdgesValues(Dictionary<TPoint, Vertex<TEdge, TPoint>> verticesDictionary)
        {
            foreach (var vertex in verticesDictionary.Values)
            {
                var halfEdgesList =
                    vertex
                        .HalfEdges
                        .Cast<HalfEdge<TEdge, TPoint>>()
                        .OrderBy(
                            he => he.OriginalSegment,
                            _coincidentEdgeComparer)
                        .ToList();

                for (int i = 0; i < halfEdgesList.Count - 1; i++)
                {
                    var e1 = halfEdgesList[i];
                    var e2 = halfEdgesList[i + 1];

                    if (e1.Twin is null)
                    {
                        throw new InvalidOperationException(
                            $"There was no twin attached to half edge {e1}.");
                    }

                    e1.Twin.Next = e2;
                    e2.Previous = e1.Twin;
                }

                var he1 = halfEdgesList.Last();
                var he2 = halfEdgesList.First();

                if (he1.Twin is null)
                {
                    throw new InvalidOperationException(
                        $"There was no twin attached to half edge {he1}.");
                }

                he1.Twin.Next = he2;
                he2.Previous = he1.Twin;
            }
        }

        private static List<Face<TEdge, TPoint>> CreateFaces(
            List<HalfEdge<TEdge, TPoint>> halfEdges)
        {
            var faces = new List<Face<TEdge, TPoint>>();

            foreach (var halfEdge in halfEdges)
            {
                if (halfEdge.Face == null)
                {
                    var face = new Face<TEdge, TPoint>();

                    var currentHalfEdge = halfEdge;

                    while (currentHalfEdge.Next != halfEdge)
                    {
                        currentHalfEdge.Face = face;
                        face.HalfEdges.Add(currentHalfEdge);

                        if (currentHalfEdge.Next is null)
                        {
                            throw new InvalidOperationException(
                                $"The half edge {currentHalfEdge} has a null next edge.");
                        }
                        currentHalfEdge = currentHalfEdge.Next;
                    }
                    currentHalfEdge.Face = face;
                    face.HalfEdges.Add(currentHalfEdge);

                    faces.Add(face);
                }
            }

            return faces;
        }
    }
}
