using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class HalfEdge<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public HalfEdge(TEdge segment, Vertex<TEdge, TPoint> origin, HalfEdge<TEdge, TPoint>? twin = null)
        {
            OriginalSegment = segment ?? throw new ArgumentNullException(nameof(segment));
            Origin = origin ?? throw new ArgumentNullException(nameof(origin));
            Twin = twin;
        }

        public TEdge OriginalSegment { get; }
        public Vertex<TEdge, TPoint> Origin { get; }
        public HalfEdge<TEdge, TPoint>? Twin { get; set; }

        internal void SetTwin(HalfEdge<TEdge, TPoint> twin)
        {
            Twin = twin ?? throw new ArgumentNullException(nameof(twin));
        }
    }
}