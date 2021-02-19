using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public interface IHalfEdge<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public TEdge OriginalSegment { get; }

        public IVertex<TEdge, TPoint> Origin { get; }

        public IHalfEdge<TEdge, TPoint> Twin { get; }

        public IHalfEdge<TEdge, TPoint> Next { get; }

        public IHalfEdge<TEdge, TPoint> Previous { get; }

        public IFace<TEdge, TPoint> Face { get; }
    }
}
