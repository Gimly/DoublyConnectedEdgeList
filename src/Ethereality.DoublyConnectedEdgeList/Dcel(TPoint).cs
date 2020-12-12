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
            IEnumerable<InternalVertex<TEdge, TPoint>> vertices,
            IEnumerable<InternalHalfEdge<TEdge, TPoint>> halfEdges,
            IEnumerable<InternalFace<TEdge, TPoint>> faces)
        {
            Vertices =
                vertices?.Select(v => v.ToVertex()).ToList()
                ?? throw new ArgumentNullException(nameof(vertices));

            HalfEdges =
                halfEdges?.Select(h => h.ToHalfEdge()).ToList()
                ?? throw new ArgumentNullException(nameof(halfEdges));

            Faces =
                faces?.Select(f => f.ToFace()).ToList()
                ?? throw new ArgumentNullException(nameof(faces));
        }

        public IReadOnlyList<Vertex<TEdge, TPoint>> Vertices { get; }

        public IReadOnlyList<HalfEdge<TEdge, TPoint>> HalfEdges { get; }

        public IReadOnlyList<Face<TEdge, TPoint>> Faces { get; }
    }
}