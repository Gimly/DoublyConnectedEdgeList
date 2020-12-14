using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    internal class InternalHalfEdge<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public InternalHalfEdge(TEdge segment, InternalVertex<TEdge, TPoint> origin)
        {
            OriginalSegment = segment ?? throw new ArgumentNullException(nameof(segment));
            Origin = origin ?? throw new ArgumentNullException(nameof(origin));
        }

        public TEdge OriginalSegment { get; }

        public InternalVertex<TEdge, TPoint> Origin { get; }

        public InternalHalfEdge<TEdge, TPoint>? Twin { get; set; }

        public InternalHalfEdge<TEdge, TPoint>? Next { get; set; }

        public InternalHalfEdge<TEdge, TPoint>? Previous { get; set; }

        public InternalFace<TEdge, TPoint>? Face { get; set; }

        public override string ToString()
        {
            if (!(Next is null))
            {
                return $"Half edge: {Origin.OriginalPoint} -> {Next.Origin.OriginalPoint}";
            }

            return $"Half edge: {Origin.OriginalPoint} -> ()";
        }
    }
}