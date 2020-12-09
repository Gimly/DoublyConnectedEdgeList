using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ethereality.DoublyConnectedEdgeList
{
    public static class Dcel
    {
        public static Dcel<TPoint> FromShape<TPoint>(IEnumerable<ISegment<TPoint>> segments) 
            where TPoint : IComparable<TPoint>, IEquatable<TPoint>
        {
            var vertices = 
                segments.Select(s => s.PointA)
                        .Concat(segments.Select(s => s.PointB))
                        .Distinct()
                        .Select(point => new Vertex<TPoint>(point));

            return new Dcel<TPoint>(vertices);
        }
    }
}
