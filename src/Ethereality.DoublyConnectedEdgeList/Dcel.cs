using System;
using System.Collections.Generic;
using System.Linq;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class Dcel<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        internal Dcel(
            IEnumerable<Vertex<TEdge, TPoint>> vertices,
            IEnumerable<HalfEdge<TEdge, TPoint>> halfEdges,
            IEnumerable<Face<TEdge, TPoint>> faces)
        {
            Vertices =
                vertices?.ToList() ?? throw new ArgumentNullException(nameof(vertices));

            HalfEdges =
                halfEdges?.ToList() ?? throw new ArgumentNullException(nameof(halfEdges));

            Faces =
                faces?.ToList() ?? throw new ArgumentNullException(nameof(faces));
        }

        public HalfEdge<TEdge, TPoint>? FindHalfEdge(TPoint pointA, TPoint pointB)
        {
            return HalfEdges.SingleOrDefault(
                halfEdge =>
                    halfEdge.Origin.OriginalPoint.Equals(pointA) &&
                    halfEdge.Next.Origin.OriginalPoint.Equals(pointB));
        }

        public IReadOnlyList<Vertex<TEdge, TPoint>> Vertices { get; }

        public IReadOnlyList<HalfEdge<TEdge, TPoint>> HalfEdges { get; }

        public IReadOnlyList<Face<TEdge, TPoint>> Faces { get; }
    }
}