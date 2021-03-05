using System;
using System.Collections.Generic;

namespace Ethereality.DoublyConnectedEdgeList
{
    internal class Face<TEdge, TPoint> : IFace<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public Face() => HalfEdges = new List<HalfEdge<TEdge, TPoint>>();

        public List<HalfEdge<TEdge, TPoint>> HalfEdges { get; }

        IEnumerable<IHalfEdge<TEdge, TPoint>> IFace<TEdge, TPoint>.HalfEdges =>
            HalfEdges ?? throw new InvalidOperationException("Half edge must be set.");
    }
}
