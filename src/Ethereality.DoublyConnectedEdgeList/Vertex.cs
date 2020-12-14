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
            IEnumerable<HalfEdge<TEdge, TPoint>> halfEdges)
        {
            OriginalPoint = originalPoint;
            HalfEdges = halfEdges?.ToList() ?? throw new ArgumentNullException(nameof(halfEdges));
        }

        public TPoint OriginalPoint { get; }

        public IReadOnlyList<HalfEdge<TEdge, TPoint>> HalfEdges { get; }
    }
}
