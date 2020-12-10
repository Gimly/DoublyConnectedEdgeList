using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ethereality.DoublyConnectedEdgeList
{
    public static class Dcel
    {
        public static Dcel<TEdge, TPoint> FromShape<TEdge, TPoint>(IEnumerable<TEdge> edges) 
            where TPoint : IEquatable<TPoint>
            where TEdge : IEdge<TPoint>
        {
            var verticesDictionary = 
                edges.Select(s => s.PointA)
                     .Concat(edges.Select(s => s.PointB))
                     .Distinct()
                     .Select(point => new Vertex<TEdge, TPoint>(point))
                     .ToDictionary(value => value.OriginalPoint, value => value);

            var halfEdges = new List<HalfEdge<TEdge, TPoint>>();

            foreach (var segment in edges)
            {
                var pointAVertex = verticesDictionary[segment.PointA];
                var firstHalfEdge = new HalfEdge<TEdge, TPoint>(segment, pointAVertex);
                if (pointAVertex.IncidentEdge is null)
                {
                    pointAVertex.SetIncidentEdge(firstHalfEdge);
                }
                
                var pointBVertex = verticesDictionary[segment.PointB];
                var secondHalfEdge = new HalfEdge<TEdge, TPoint>(segment, pointBVertex, firstHalfEdge);
                pointBVertex.SetIncidentEdge(secondHalfEdge);
                
                if (pointBVertex.IncidentEdge is null)
                {
                    pointBVertex.SetIncidentEdge(firstHalfEdge);
                }

                firstHalfEdge.SetTwin(secondHalfEdge);

                halfEdges.Add(firstHalfEdge);
                halfEdges.Add(secondHalfEdge);
            }

            return new Dcel<TEdge, TPoint>(verticesDictionary.Values, halfEdges);
        }
    }
}
