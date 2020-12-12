using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class Face<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        internal Face(InternalHalfEdge<TEdge, TPoint> halfEdge)
        {
            HalfEdge = halfEdge.ToHalfEdge();
        }

        public HalfEdge<TEdge, TPoint> HalfEdge { get; }
    }
}
