using System;
using System.Collections.Generic;
using System.Linq;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class Vertex<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        internal Vertex(
            TPoint originalPoint,
            List<InternalHalfEdge<TEdge, TPoint>> internalHalfEdges)
        {
            OriginalPoint = originalPoint;
            HalfEdges = internalHalfEdges.Select(h => h.ToHalfEdge()).ToList();
        }

        public TPoint OriginalPoint { get; }

        public IReadOnlyList<HalfEdge<TEdge, TPoint>> HalfEdges { get; }
    }
}
