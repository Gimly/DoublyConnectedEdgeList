using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class HalfEdge<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        internal HalfEdge(
            TEdge originalSegment,
            Vertex<TEdge, TPoint> origin,
            HalfEdge<TEdge, TPoint> twin,
            HalfEdge<TEdge, TPoint> next,
            HalfEdge<TEdge, TPoint> previous,
            Face<TEdge, TPoint>? face)
        {
            OriginalSegment = originalSegment;
            Origin = origin;
            Twin = twin;
            Next = next;
            Previous = previous;
            Face = face;
        }

        public TEdge OriginalSegment { get; }

        public Vertex<TEdge, TPoint> Origin { get; }

        public HalfEdge<TEdge, TPoint> Twin { get; }

        public HalfEdge<TEdge, TPoint> Next { get; }

        public HalfEdge<TEdge, TPoint> Previous { get; }

        public Face<TEdge, TPoint>? Face { get; }
    }
}
