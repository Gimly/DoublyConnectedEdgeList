using System;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class Vertex<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        public Vertex(TPoint point)
        {
            OriginalPoint = point ?? throw new ArgumentNullException(nameof(point));
        }

        public TPoint OriginalPoint { get; }

        public HalfEdge<TEdge, TPoint>? Leaving { get; private set; }

        internal void SetIncidentEdge(HalfEdge<TEdge, TPoint> leaving)
        {
            Leaving = leaving ?? throw new ArgumentNullException(nameof(leaving));
        }
    }
}