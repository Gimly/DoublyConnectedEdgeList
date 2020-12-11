using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ethereality.DoublyConnectedEdgeList
{
    public static class Dcel
    {
        public static Dcel<TEdge, TPoint> FromShape<TEdge, TPoint>(IEnumerable<TEdge> edges, IComparer<TEdge> coincidentEdgeComparer)
            where TPoint : IEquatable<TPoint>
            where TEdge : IEdge<TPoint>
        {
            var verticesDictionary =
                edges.Select(s => s.PointA)
                     .Concat(edges.Select(s => s.PointB))
                     .Distinct()
                     .Select(point => new Vertex<TEdge, TPoint>(point))
                     .ToDictionary(value => value.OriginalPoint, value => value);

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

            foreach (var vertex in verticesDictionary.Values)
            {
                var halfEdgesList = vertex.HalfEdges.OrderBy(he => he.OriginalSegment, coincidentEdgeComparer).ToList();

                for (int i = 0; i < halfEdgesList.Count - 1; i++)
                {
                    var e1 = halfEdgesList[i];
                    var e2 = halfEdgesList[i + 1];

                    e1.Twin.Next = e2;
                    e2.Previous = e1.Twin;
                }

                var he1 = halfEdgesList.Last();
                var he2 = halfEdgesList.First();

                he1.Twin.Next = he2;
                he2.Previous = he1.Twin;
            }

            var faces = new List<Face<TEdge, TPoint>>();
            foreach (var halfEdge in halfEdges)
            {
                if (halfEdge.Face == null)
                {
                    var face = new Face<TEdge, TPoint>()
                    {
                        HalfEdge = halfEdge
                    };

                    var currentHalfEdge = halfEdge;

                    while (currentHalfEdge.Next != halfEdge)
                    {
                        currentHalfEdge.Face = face;
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
