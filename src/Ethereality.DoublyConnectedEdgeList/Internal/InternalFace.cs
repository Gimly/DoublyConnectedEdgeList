using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    internal class InternalFace<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public InternalHalfEdge<TEdge, TPoint>? HalfEdge { get; set; }
    }
}