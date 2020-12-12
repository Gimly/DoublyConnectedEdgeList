using System;
using System.Collections.Generic;

namespace Ethereality.DoublyConnectedEdgeList
{
    internal class InternalVertex<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public InternalVertex(TPoint point)
        {
            OriginalPoint = point ?? throw new ArgumentNullException(nameof(point));
            HalfEdges = new List<InternalHalfEdge<TEdge, TPoint>>();
        }

        public TPoint OriginalPoint { get; }

        public List<InternalHalfEdge<TEdge, TPoint>> HalfEdges { get; }

        public override string ToString() => $"Vertex: {OriginalPoint}";

        internal Vertex<TEdge, TPoint> ToVertex() =>
            new Vertex<TEdge, TPoint>(OriginalPoint, HalfEdges);
    }
}