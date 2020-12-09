using System;
using System.Collections.Generic;
using System.Linq;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class Dcel<TPoint> where TPoint : IComparable<TPoint>
    {
        internal Dcel(IEnumerable<Vertex<TPoint>> vertices)
        {
            Vertices = vertices?.ToList() ?? throw new ArgumentNullException(nameof(vertices));
        }

        public IReadOnlyList<Vertex<TPoint>> Vertices { get; }
    }
}