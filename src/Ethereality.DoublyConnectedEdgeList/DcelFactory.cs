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

        public DcelFactory(IComparer<TEdge> coincidentEdgeComparer)
        {
            _coincidentEdgeComparer =
                coincidentEdgeComparer ?? throw new ArgumentNullException(nameof(coincidentEdgeComparer));
        }

        public Dcel<TEdge, TPoint> FromShape(IEnumerable<TEdge> edges)
        {
            var verticesDictionary =
                edges.Select(s => s.PointA)
                     .Concat(edges.Select(s => s.PointB))
                     .Distinct()
                     .Select(point => new InternalVertex<TEdge, TPoint>(point))
                     .ToDictionary(value => value.OriginalPoint, value => value);

            var halfEdges = new List<InternalHalfEdge<TEdge, TPoint>>();

            foreach (var segment in edges)
            {
                var pointAVertex = verticesDictionary[segment.PointA];
                var firstHalfEdge = new InternalHalfEdge<TEdge, TPoint>(segment, pointAVertex);
                pointAVertex.HalfEdges.Add(firstHalfEdge);

                var pointBVertex = verticesDictionary[segment.PointB];
                var secondHalfEdge = new InternalHalfEdge<TEdge, TPoint>(segment, pointBVertex)
                {
                    Twin = firstHalfEdge
                };

                pointBVertex.HalfEdges.Add(secondHalfEdge);

                firstHalfEdge.Twin = secondHalfEdge;

                halfEdges.Add(firstHalfEdge);
                halfEdges.Add(secondHalfEdge);
            }

            foreach (var vertex in verticesDictionary.Values)
            {
                var halfEdgesList =
                    vertex
                        .HalfEdges
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

            var faces = new List<InternalFace<TEdge, TPoint>>();
            foreach (var halfEdge in halfEdges)
            {
                if (halfEdge.Face == null)
                {
                    var face = new InternalFace<TEdge, TPoint>()
                    {
                        HalfEdge = halfEdge
                    };

                    var currentHalfEdge = halfEdge;

                    while (currentHalfEdge.Next != halfEdge)
                    {
                        currentHalfEdge.Face = face;

                        if (currentHalfEdge.Next is null)
                        {
                            throw new InvalidOperationException(
                                $"The half edge {currentHalfEdge} has a null next edge.");
                        }
                        currentHalfEdge = currentHalfEdge.Next;
                    }
                    currentHalfEdge.Face = face;

                    faces.Add(face);
                }
            }

            return new Dcel<TEdge, TPoint>(verticesDictionary.Values, halfEdges, faces);
        }
    }
}
