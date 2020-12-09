using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public interface ISegment<TPoint> : IComparable<ISegment<TPoint>>
        where TPoint : IComparable<TPoint>, IEquatable<TPoint>
    {
        TPoint PointA { get; }

        TPoint PointB { get; }
    }
}
