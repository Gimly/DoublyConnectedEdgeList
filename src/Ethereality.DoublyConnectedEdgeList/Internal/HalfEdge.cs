using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    internal class HalfEdge<TEdge, TPoint> : IHalfEdge<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public HalfEdge(TEdge segment, Vertex<TEdge, TPoint> origin)
        {
            OriginalSegment = segment ?? throw new ArgumentNullException(nameof(segment));
            Origin = origin ?? throw new ArgumentNullException(nameof(origin));
        }

        public TEdge OriginalSegment { get; }

        public Vertex<TEdge, TPoint> Origin { get; }

        public HalfEdge<TEdge, TPoint>? Twin { get; set; }

        public HalfEdge<TEdge, TPoint>? Next { get; set; }

        public HalfEdge<TEdge, TPoint>? Previous { get; set; }

        public Face<TEdge, TPoint>? Face { get; set; }

        IVertex<TEdge, TPoint> IHalfEdge<TEdge, TPoint>.Origin => Origin;

        IHalfEdge<TEdge, TPoint> IHalfEdge<TEdge, TPoint>.Twin =>
            Twin ?? throw new InvalidOperationException("A half edge must have a twin.");

        IHalfEdge<TEdge, TPoint> IHalfEdge<TEdge, TPoint>.Next =>
            Next ?? throw new InvalidOperationException("A half edge must have a next segment.");

        IHalfEdge<TEdge, TPoint> IHalfEdge<TEdge, TPoint>.Previous =>
            Previous ?? throw new InvalidOperationException("A half edge must have a previous segment.");

        IFace<TEdge, TPoint> IHalfEdge<TEdge, TPoint>.Face =>
            Face ?? throw new InvalidOperationException("A half edge must have a face.");

        public override string ToString() =>
            !(Next is null) 
                ? $"Half edge: {Origin.OriginalPoint} -> {Next.Origin.OriginalPoint}" 
                : $"Half edge: {Origin.OriginalPoint} -> ()";
    }
}
