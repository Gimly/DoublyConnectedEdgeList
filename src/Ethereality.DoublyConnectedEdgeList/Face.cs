using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class Face<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        internal Face(HalfEdge<TEdge, TPoint> halfEdge)
        {
            HalfEdge = halfEdge ?? throw new ArgumentNullException(nameof(halfEdge));
        }

        public HalfEdge<TEdge, TPoint> HalfEdge { get; }
    }
}
