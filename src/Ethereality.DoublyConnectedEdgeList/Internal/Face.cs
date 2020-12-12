using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class Face<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public HalfEdge<TEdge, TPoint> HalfEdge { get; internal set; }
    }
}