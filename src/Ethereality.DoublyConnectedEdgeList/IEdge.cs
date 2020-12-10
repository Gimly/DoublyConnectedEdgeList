using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public interface IEdge<TPoint> where TPoint : IEquatable<TPoint>
    {
        TPoint PointA { get; }

        TPoint PointB { get; }
    }
}
