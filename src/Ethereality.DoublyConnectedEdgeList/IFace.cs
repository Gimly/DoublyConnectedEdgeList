using System;
using System.Collections.Generic;

namespace Ethereality.DoublyConnectedEdgeList
{
    public interface IFace<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public IEnumerable<IHalfEdge<TEdge, TPoint>> HalfEdges { get; }
    }
}
