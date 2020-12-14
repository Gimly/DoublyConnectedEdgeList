using System;
using System.Collections.Generic;
using System.Linq;

namespace Ethereality.DoublyConnectedEdgeList
{
    public class DcelFactory<TEdge, TPoint>
        where TEdge : IEdge<TPoint>
        where TPoint : IEquatable<TPoint>
    {
        private readonly IComparer<TEdge> _coincidentEdgeComparer;

        private readonly IDictionary<InternalHalfEdge<TEdge, TPoint>, HalfEdge<TEdge, TPoint>> _createdHalfEdges =
            new Dictionary<InternalHalfEdge<TEdge, TPoint>, HalfEdge<TEdge, TPoint>>();

        private readonly IDictionary<InternalVertex<TEdge, TPoint>, Vertex<TEdge, TPoint>> _createdVertex =
            new Dictionary<InternalVertex<TEdge, TPoint>, Vertex<TEdge, TPoint>>();

        private readonly IDictionary<InternalFace<TEdge, TPoint>, Face<TEdge, TPoint>> _createdFaces =
            new Dictionary<InternalFace<TEdge, TPoint>, Face<TEdge, TPoint>>();

        public DcelFactory(IComparer<TEdge> coincidentEdgeComparer) =>
            _coincidentEdgeComparer =
                coincidentEdgeComparer ?? throw new ArgumentNullException(nameof(coincidentEdgeComparer));

        public Dcel<TEdge, TPoint> FromShape(IEnumerable<TEdge> edges)
        {
            var verticesDictionary = CreateVertices(edges);

            var halfEdges = CreateHalfEdges(edges, verticesDictionary);
            List<InternalFace<TEdge, TPoint>> faces = CreateFaces(halfEdges);

            return new Dcel<TEdge, TPoint>(
                verticesDictionary
                    .Values
                    .Select(v => ToVertex(v)),
                halfEdges.Select(he => ToHalfEdge(he)), 
                faces.Select(f => ToFace(f)));
        }

        private Vertex<TEdge, TPoint> ToVertex(InternalVertex<TEdge, TPoint> internalVertex)
        {
            if (_createdVertex.ContainsKey(internalVertex))
            {
                return _createdVertex[internalVertex];
            }

            var newVertex =  
                new Vertex<TEdge, TPoint>(
                    internalVertex.OriginalPoint,
                    internalVertex.HalfEdges.Select(he => ToHalfEdge(he)));

            _createdVertex.Add(internalVertex, newVertex);

            return newVertex;
        }

        private HalfEdge<TEdge, TPoint> ToHalfEdge(InternalHalfEdge<TEdge, TPoint> internalHalfEdge)
        {
            if (_createdHalfEdges.ContainsKey(internalHalfEdge))
            {
                return _createdHalfEdges[internalHalfEdge];
            }

            var newHalfEdge =
                new HalfEdge<TEdge, TPoint>(
                    internalHalfEdge.OriginalSegment,
                    ToVertex(internalHalfEdge.Origin),
                    ToHalfEdge(internalHalfEdge.Twin ??
                        throw new InvalidOperationException("Cannot create a half edge with null twin.")),
                    ToHalfEdge(internalHalfEdge.Next ??
                        throw new InvalidOperationException("Cannot create a half edge with null next.")),
                    ToHalfEdge(internalHalfEdge.Previous ??
                        throw new InvalidOperationException("Cannot create a half edge with null previous.")),
                    ToFace(internalHalfEdge.Face ??
                        throw new InvalidOperationException("Cannot create a half edge with null face.")));

            _createdHalfEdges.Add(internalHalfEdge, newHalfEdge);

            return newHalfEdge;
        }

        private Face<TEdge, TPoint> ToFace(InternalFace<TEdge, TPoint> internalFace)
        {
            if (_createdFaces.ContainsKey(internalFace))
            {
                return _createdFaces[internalFace];
            }

            var newFace = new Face<TEdge, TPoint>(
                ToHalfEdge(internalFace.HalfEdge ??
                    throw new InvalidOperationException("Cannot create a face without a half edge.")));

            _createdFaces.Add(internalFace, newFace);

            return newFace;
        }

        private static Dictionary<TPoint, InternalVertex<TEdge, TPoint>> CreateVertices(IEnumerable<TEdge> edges) =>
            edges.Select(s => s.PointA)
                 .Concat(edges.Select(s => s.PointB))
                 .Distinct()
                 .Select(point => new InternalVertex<TEdge, TPoint>(point))
                 .ToDictionary(value => value.OriginalPoint, value => value);

        private List<InternalHalfEdge<TEdge, TPoint>> CreateHalfEdges(
            IEnumerable<TEdge> edges,
            Dictionary<TPoint, InternalVertex<TEdge, TPoint>> verticesDictionary)
        {
            var halfEdges = new List<InternalHalfEdge<TEdge, TPoint>>();

            foreach (var segment in edges)
            {
                var pointAVertex = verticesDictionary[segment.PointA];
                var firstHalfEdge = new InternalHalfEdge<TEdge, TPoint>(segment, pointAVertex);
                pointAVertex.HalfEdges.Add(firstHalfEdge);

                var pointBVertex = verticesDictionary[segment.PointB];
                var secondHalfEdge = new InternalHalfEdge<TEdge, TPoint>(segment, pointBVertex)
                {
                    Twin = firstHalfEdge
                };

                pointBVertex.HalfEdges.Add(secondHalfEdge);

                firstHalfEdge.Twin = secondHalfEdge;

                halfEdges.Add(firstHalfEdge);
                halfEdges.Add(secondHalfEdge);
            }

            SetHalfEdgesValues(verticesDictionary);

            return halfEdges;
        }

        private void SetHalfEdgesValues(Dictionary<TPoint, InternalVertex<TEdge, TPoint>> verticesDictionary)
        {
            foreach (var vertex in verticesDictionary.Values)
            {
                var halfEdgesList =
                    vertex
                        .HalfEdges
                        .OrderBy(
                            he => he.OriginalSegment,
                            _coincidentEdgeComparer)
                        .ToList();

                for (int i = 0; i < halfEdgesList.Count - 1; i++)
                {
                    var e1 = halfEdgesList[i];
                    var e2 = halfEdgesList[i + 1];

                    if (e1.Twin is null)
                    {
                        throw new InvalidOperationException(
                            $"There was no twin attached to half edge {e1}.");
                    }

                    e1.Twin.Next = e2;
                    e2.Previous = e1.Twin;
                }

                var he1 = halfEdgesList.Last();
                var he2 = halfEdgesList.First();

                if (he1.Twin is null)
                {
                    throw new InvalidOperationException(
                        $"There was no twin attached to half edge {he1}.");
                }

                he1.Twin.Next = he2;
                he2.Previous = he1.Twin;
            }
        }

        private static List<InternalFace<TEdge, TPoint>> CreateFaces(
            List<InternalHalfEdge<TEdge, TPoint>> halfEdges)
        {
            var faces = new List<InternalFace<TEdge, TPoint>>();

            foreach (var halfEdge in halfEdges)
            {
                if (halfEdge.Face == null)
                {
                    var face = new InternalFace<TEdge, TPoint>()
                    {
                        HalfEdge = halfEdge
                    };

                    var currentHalfEdge = halfEdge;

                    while (currentHalfEdge.Next != halfEdge)
                    {
                        currentHalfEdge.Face = face;

                        if (currentHalfEdge.Next is null)
                        {
                            throw new InvalidOperationException(
                                $"The half edge {currentHalfEdge} has a null next edge.");
                        }
                        currentHalfEdge = currentHalfEdge.Next;
                    }
                    currentHalfEdge.Face = face;

                    faces.Add(face);
                }
            }

            return faces;
        }
    }
}
