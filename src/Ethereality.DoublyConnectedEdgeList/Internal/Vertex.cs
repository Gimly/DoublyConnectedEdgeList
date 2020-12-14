using System;
using System.Collections.Generic;

namespace Ethereality.DoublyConnectedEdgeList
{
    internal class Vertex<TEdge, TPoint> : IVertex<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public Vertex(TPoint point)
        {
            OriginalPoint = point ?? throw new ArgumentNullException(nameof(point));
            HalfEdges = new List<IHalfEdge<TEdge, TPoint>>();
        }

        public TPoint OriginalPoint { get; }

        public List<IHalfEdge<TEdge, TPoint>> HalfEdges { get; }

        IReadOnlyList<IHalfEdge<TEdge, TPoint>> IVertex<TEdge, TPoint>.HalfEdges =>
            HalfEdges;

        public override string ToString() => $"Vertex: {OriginalPoint}";
    }
}