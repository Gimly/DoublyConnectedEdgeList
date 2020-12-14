using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    internal class Face<TEdge, TPoint> : IFace<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public HalfEdge<TEdge, TPoint>? HalfEdge { get; set; }

        IHalfEdge<TEdge, TPoint> IFace<TEdge, TPoint>.HalfEdge =>
            HalfEdge ?? throw new InvalidOperationException("Half edge must be set.");
    }
}