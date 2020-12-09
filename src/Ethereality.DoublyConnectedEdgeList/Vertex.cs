namespace Ethereality.DoublyConnectedEdgeList
{
    public class Vertex<TPoint>
    {
        public Vertex(TPoint point)
        {
            OriginalPoint = point;
        }

        public TPoint OriginalPoint { get; }
    }
}