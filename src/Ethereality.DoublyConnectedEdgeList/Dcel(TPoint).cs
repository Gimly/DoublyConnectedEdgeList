using System;
using System.Collections.Generic;
using System.Linq;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class Dcel<TEdge, TPoint> 
        where TEdge: IEdge<TPoint>
        where TPoint: IEquatable<TPoint>
    {
        internal Dcel(IEnumerable<Vertex<TEdge, TPoint>> vertices, IEnumerable<HalfEdge<TEdge, TPoint>> halfEdges)
        {
            Vertices = vertices?.ToList() ?? throw new ArgumentNullException(nameof(vertices));
            HalfEdges = halfEdges?.ToList() ?? throw new ArgumentNullException(nameof(halfEdges));
        }

        public IReadOnlyList<Vertex<TEdge, TPoint>> Vertices { get; }

        public IReadOnlyList<HalfEdge<TEdge, TPoint>> HalfEdges { get; }
    }
}