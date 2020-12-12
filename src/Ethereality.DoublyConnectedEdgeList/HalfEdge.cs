using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class HalfEdge<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        internal HalfEdge(
            TEdge originalSegment,
            InternalVertex<TEdge, TPoint> origin,
            InternalHalfEdge<TEdge, TPoint> twin,
            InternalHalfEdge<TEdge, TPoint>? next,
            InternalHalfEdge<TEdge, TPoint>? previous,
            InternalFace<TEdge, TPoint>? face)
        {
            OriginalSegment = originalSegment;
            Origin = origin.ToVertex();
            Twin = twin.ToHalfEdge();
            Next = next?.ToHalfEdge();
            Previous = previous?.ToHalfEdge();
            Face = face?.ToFace();
        }

        public TEdge OriginalSegment { get; }

        public Vertex<TEdge, TPoint> Origin { get; }

        public HalfEdge<TEdge, TPoint> Twin { get; }

        public HalfEdge<TEdge, TPoint>? Next { get; }

        public HalfEdge<TEdge, TPoint>? Previous { get; }

        public Face<TEdge, TPoint>? Face { get; }
    }
}
