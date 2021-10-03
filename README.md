# C# Doubly Connected Edge List
Implements a doubly connected edge list (DCEL) in C#.

From Wikipedia: The doubly connected edge list (DCEL), also known as half-edge data structure, is a data structure to represent an embedding of a planar graph in the plane, and polytopes in 3D. This data structure provides efficient manipulation of the topological information associated with the objects in question (vertices, edges, faces). It is used in many algorithms of computational geometry to handle polygonal subdivisions of the plane, commonly called planar straight-line graphs (PSLG). For example, a Voronoi diagram is commonly represented by a DCEL inside a bounding box.

## Usage

The goal of this implementation is to be very open on its usage. To start, it only requires to have a Shape represented by a list of Edges and an IComparer that will be used to compare the coincident and order them geometrically. 

### Edges

The Edges are represented by an interface called `IEdge` which only needs two "points" which have the only requirement to implement `IEquatable`. This allows to define the points as you see fit. It can be 2D, 3D, or even 4D :).

### IComparer<TEdge> coincidentEdgeComparer

The other thing needed to create the DCEL is a class that implements `IComparer<TEdge>` where `TEdge` is your definition of an edge. This is needed to define in which "order" we should take the edges that are coincident with a point. This comparer will be called at each point and will define the order in which the edges will be used in the algorithm.
