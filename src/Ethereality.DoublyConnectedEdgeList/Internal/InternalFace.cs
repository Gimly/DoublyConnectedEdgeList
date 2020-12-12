using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    internal class InternalFace<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public InternalHalfEdge<TEdge, TPoint>? HalfEdge { get; internal set; }

        public Face<TEdge, TPoint> ToFace() =>
            new Face<TEdge, TPoint>(
                HalfEdge ??  throw new InvalidOperationException("Cannot create a face without a linked half edge."));
    }
}