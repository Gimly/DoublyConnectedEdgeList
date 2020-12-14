using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public interface IFace<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public IHalfEdge<TEdge, TPoint> HalfEdge { get; }
    }
}
