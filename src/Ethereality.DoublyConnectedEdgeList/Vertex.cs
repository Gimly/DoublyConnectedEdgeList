using System;
using System.Collections.Generic;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class Vertex<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public Vertex(TPoint point)
        {
            OriginalPoint = point ?? throw new ArgumentNullException(nameof(point));
            HalfEdges = new List<HalfEdge<TEdge, TPoint>>();
        }

        public TPoint OriginalPoint { get; }

        public List<HalfEdge<TEdge, TPoint>> HalfEdges { get; }

        public override string ToString() => $"Vertex: {OriginalPoint}";
    }
}