using System;
using System.Collections.Generic;

namespace Ethereality.DoublyConnectedEdgeList
{
    public interface IVertex<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public TPoint OriginalPoint { get; }

        public IReadOnlyList<IHalfEdge<TEdge, TPoint>> HalfEdges { get; }
    }
}
