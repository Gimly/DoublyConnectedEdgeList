using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class HalfEdge<TEdge, TPoint>
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

        public HalfEdge<TEdge, TPoint> Previous { get; set; }

        public Face<TEdge, TPoint> Face { get; set; }

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