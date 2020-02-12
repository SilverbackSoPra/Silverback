using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Collision;
using Microsoft.Xna.Framework;

namespace LevelEditor.Pathfinding
{
    [Serializable()]
    public class VisibilityGraph: ISerializable
    {

        public static List<Edge> mDrawable = new List<Edge>();

        public List<Edge> mEdges;
        public List<Vertex> mVertices;

        private List<Edge> mObstacleEdges;
        private List<CollisionRectangle> mCollision;

        public VisibilityGraph(List<CollisionRectangle> staticCollisionRectangles, Rectangle boundary, float offset, bool bruteForce)
        {

            // We should instead use arrays for better performance.
            // Especially because foreach loops are way slower when using lists
            mVertices = new List<Vertex>();
            mCollision = staticCollisionRectangles;
            mObstacleEdges = new List<Edge>();
            var edgesHashSet = new HashSet<Edge>(new CompareEdgeVertices());
            var verticesDictionary = new Dictionary<Vector2, Vertex>();

            mCollision = new List<CollisionRectangle>();

            foreach (var rectangle in staticCollisionRectangles)
            {

                var midPoint = rectangle.V1Transformed + (rectangle.V2Transformed - rectangle.V1Transformed) * 0.5f + (rectangle.V3Transformed - rectangle.V1Transformed) * 0.5f;
                var v1 = rectangle.V1Transformed + Vector2.Normalize((rectangle.V1Transformed - midPoint)) * offset;
                var v2 = rectangle.V2Transformed + Vector2.Normalize((rectangle.V2Transformed - midPoint)) * offset;
                var v3 = rectangle.V3Transformed + Vector2.Normalize((rectangle.V3Transformed - midPoint)) * offset;

                mCollision.Add(new CollisionRectangle(v1, v2, v3));

            }


            foreach (var rectangle in mCollision)
            {

                var v1 = new Vertex(rectangle.V1Transformed, rectangle);
                var v2 = new Vertex(rectangle.V2Transformed, rectangle);
                var v3 = new Vertex(rectangle.V3Transformed, rectangle);
                var v4 = new Vertex(rectangle.V4Transformed, rectangle);

                var edge1 = new Edge(v1, v2, 0.0f);
                var edge2 = new Edge(v1, v3, 0.0f);
                var edge3 = new Edge(v3, v4, 0.0f);
                var edge4 = new Edge(v2, v4, 0.0f);

                v1.Edge1 = edge1;
                v1.Edge2 = edge2;

                v2.Edge1 = edge1;
                v2.Edge2 = edge4;

                v3.Edge1 = edge2;
                v3.Edge2 = edge3;

                v4.Edge1 = edge3;
                v4.Edge2 = edge4;

                mObstacleEdges.Add(edge1);
                mObstacleEdges.Add(edge2);
                mObstacleEdges.Add(edge3);
                mObstacleEdges.Add(edge4);

                mVertices.Add(v1);
                mVertices.Add(v2);
                mVertices.Add(v3);
                mVertices.Add(v4);

                if (!verticesDictionary.ContainsKey(v1.Position))
                {
                    verticesDictionary.Add(v1.Position, v1);
                }
                if (!verticesDictionary.ContainsKey(v2.Position))
                {
                    verticesDictionary.Add(v2.Position, v2);
                }
                if (!verticesDictionary.ContainsKey(v3.Position))
                {
                    verticesDictionary.Add(v3.Position, v3);
                }
                if (!verticesDictionary.ContainsKey(v4.Position))
                {
                    verticesDictionary.Add(v4.Position, v4);
                }

            }

            // Now we're searching for vertices which might be inside another rectangle
            // These vertices shouldn't be considered when we are constructing the graph later
            var quadTree = new QuadTree<CollisionRectangle>(boundary, 4, 7);

            // Insert the rectangles into the tree
            foreach (var rectangle in mCollision)
            {
                quadTree.Insert(rectangle, rectangle.GetAxisAlignedRectangle(1));
            }

            // Now foreach rectangle get the nearest rectangles which might collide with it
            foreach (var rectangle in mCollision)
            {
                var list = quadTree.QueryRectangle(rectangle.GetAxisAlignedRectangle(1));
                foreach (var r in list)
                {

                    if (r == rectangle)
                    {
                        continue;
                    }

                    // If there is an intersection, check which points are inside the other rectangle
                    if (verticesDictionary.ContainsKey(r.V1Transformed))
                    {
                        var v = verticesDictionary[r.V1Transformed];
                        v.Connectable = v.Connectable == true ? !rectangle.PointInside(v.Position) : false;
                    }
                    if (verticesDictionary.ContainsKey(r.V2Transformed))
                    {
                        var v = verticesDictionary[r.V2Transformed];
                        v.Connectable = v.Connectable == true ? !rectangle.PointInside(v.Position) : false;
                    }
                    if (verticesDictionary.ContainsKey(r.V3Transformed))
                    {
                        var v = verticesDictionary[r.V3Transformed];
                        v.Connectable = v.Connectable == true ? !rectangle.PointInside(v.Position) : false;
                    }
                    if (verticesDictionary.ContainsKey(r.V4Transformed))
                    {
                        var v = verticesDictionary[r.V4Transformed];
                        v.Connectable = v.Connectable == true ? !rectangle.PointInside(v.Position) : false;
                    }
                }
            }

            var vertices = mVertices.ToArray();

            foreach (var vertex in vertices)
            {

                if (!bruteForce)
                {
                    CalculateVisibleVertices(vertex, mVertices, ref edgesHashSet);
                }
                else
                {
                    foreach (var v in vertices)
                    {

                        var edge = new Edge(vertex, v, 0.0f);
                        Vector2 intersectionPoint = new Vector2();

                        var intersection = false;
                        foreach (var e in mObstacleEdges)
                        {
                            if (edge.Intersects(e, ref intersectionPoint))
                            {
                                intersection = true;
                                break;
                            }
                        }

                        if (!intersection)
                        {
                            edge.Weight = Vector2.Distance(vertex.Position, v.Position);
                            edgesHashSet.Add(edge);
                        }
                    }
                }

            }

            mEdges = edgesHashSet.ToList();

        }

        public Vertex GetPath(Vector3 start, Vector3 destination)
        {
            // We need to insert start and destination into the graph
            var edgeHashSet = new HashSet<Edge>(new CompareEdgeVertices());

            // We dont care about the rectanlge
            var rectagle = new CollisionRectangle(new Vector2(-1.0f), new Vector2(1.0f), new Vector2(1.0f, -1.0f));
            Vertex startVertex = new Vertex(new Vector2(start.X, start.Z), rectagle);
            Vertex destinationVertex = new Vertex(new Vector2(destination.X, destination.Z), rectagle);

            mVertices.Add(startVertex);
            mVertices.Add(destinationVertex);
            CalculateVisibleVertices(startVertex, mVertices, ref edgeHashSet);      
            CalculateVisibleVertices(destinationVertex, mVertices, ref edgeHashSet);
            mVertices.Remove(startVertex);
            mVertices.Remove(destinationVertex);

            var directEdge = new Edge(startVertex, destinationVertex, 0.0f);

            // If there exists a direct path, we dont have to use A*
            if (edgeHashSet.Contains(directEdge))
            {
                startVertex.mNextVertex = destinationVertex;
                startVertex.mNextVertex.mPreviousVertex = startVertex;
                // We need to remove the vertices which we added
                foreach (var vertex in mVertices)
                {
                    vertex.mAdjacentVertices.Remove(startVertex);
                    vertex.mAdjacentVertices.Remove(destinationVertex);
                }
                return destinationVertex;
            }
        
            // Now calculate the path with A*
            startVertex = AStar.Search(startVertex, destinationVertex, mVertices);

            // We need to remove the vertices which we added
            foreach (var vertex in mVertices)
            {
                vertex.mAdjacentVertices.Remove(startVertex);
                vertex.mAdjacentVertices.Remove(destinationVertex);
            }
                
            return startVertex.mNextVertex;

        }

        private void CalculateVisibleVertices(Vertex vertex, List<Vertex> vertices,  ref HashSet<Edge> edgesHashSet)
        {

            if (!vertex.Connectable)
            {
                return;
            }

            var center = vertex;

            foreach (var edge in mObstacleEdges)
            {
                edge.Visited = false;
            }

            // Sort the vertices in angular order
            // Calculate the angles to the center vertex
            foreach (var v in vertices)
            {
                v.AngleToCenter = center.Angle(v);
                v.DistanceToCenter = Vector2.DistanceSquared(center.Position, v.Position);
            }

            vertices.Sort(new CompareVertexAngle());

            // Create the scanline with the vertex that has the smallest angle
            var scanLine = new Edge(center, vertices[0], 0.0f);

            var set = new SortedSet<Edge>(new CompareEdgeDistance());

            Vector2 intersectionPoint = Vector2.Zero;

            // First we need to test against every edge
            foreach (var edge in mObstacleEdges)
            {
                if (edge.Intersects(scanLine, ref intersectionPoint))
                {
                    var distSquared = Vector2.DistanceSquared(intersectionPoint, center.Position);
                    if (distSquared != 0.0f)
                    {
                        edge.DistanceSquaredToCenter = distSquared;
                        set.Add(edge);
                    }
                }
            }

            // Now we need to iterate over the rest of the list
            // For each vertex we insert the adjacent edges which 
            // lie on the clockwise side of the scan line and delete
            // all adjacent egdes of the vertex which are on the 
            // counter clockwise side.
            foreach (var v in vertices)
            {

                var edge = new Edge(center, v, 0.0f);
                bool locked = false;

                if (!v.Connectable)
                {
                    locked = true;
                }

                // Check if the edge lies in the rectangle of its parents
                // If this is the case we dont want the edge to be added to
                // the graph
                if (edge.V1.ParentRectangle == edge.V2.ParentRectangle)
                {
                    var rec = edge.V1.ParentRectangle;
                    if ((edge.V1.Position == rec.V1Transformed && edge.V2.Position == rec.V4Transformed) ||
                        (edge.V1.Position == rec.V4Transformed && edge.V2.Position == rec.V1Transformed))
                    {
                        locked = true;
                    }
                    if ((edge.V1.Position == rec.V2Transformed && edge.V2.Position == rec.V3Transformed) ||
                        (edge.V1.Position == rec.V3Transformed && edge.V2.Position == rec.V2Transformed))
                    {
                        locked = true;
                    }
                }

                // We dont want to check against the center vertex. This would cause an edge from center-center
                // which might be a problem for the path finding
                if (!v.Equals(center) && !locked)
                {

                    // If the set is null the vertex must be visible
                    if (set.Min == null)
                    {
                        edge.Weight = Vector2.Distance(center.Position, v.Position);
                        center.mAdjacentVertices.Add(v);
                        v.mAdjacentVertices.Add(center);
                        edgesHashSet.Add(edge);
                    }
                    else
                    {

                        // We should just need to check if the first line intersects with the
                        // edge, but somehow this isn't working. I suppose this is because of
                        // the not correct calulcation of the nearest edge below
                        var intersection = false;
                        foreach (var e in set)
                        {
                            if (edge.Intersects(e, ref intersectionPoint))
                            {
                                intersection = true;
                                break;
                            }
                        }

                        if (!intersection)
                        {
                            edge.Weight = Vector2.Distance(center.Position, v.Position);
                            center.mAdjacentVertices.Add(v);
                            v.mAdjacentVertices.Add(center);
                            edgesHashSet.Add(edge);
                        }
                    }
                }

                if (v.Edge1 == null && v.Edge2 == null)
                {
                    continue;
                }

                var incidentVertex1 = v.Edge1.GetOtherVertex(v);
                var incidentVertex2 = v.Edge2.GetOtherVertex(v);

                // Check incident edges
                if (CheckVectorSide(center.Position, incidentVertex1.Position, v.Position.X, v.Position.Y) < 0)
                {
                    if (v.Edge1.Visited)
                    {
                        set.Remove(v.Edge1);
                    }
                }
                else
                {

                    // This is somewhat hacky. We should calculate this properly
                    var distance = Vector2.DistanceSquared(center.Position, v.Position + (incidentVertex1.Position - v.Position) / 2.0f);

                    if (distance > 0.0f)
                    {
                        v.Edge1.DistanceSquaredToCenter = distance;
                        v.Edge1.Visited = true;
                        set.Add(v.Edge1);
                    }
                }

                if (CheckVectorSide(center.Position, incidentVertex2.Position, v.Position.X, v.Position.Y) < 0)
                {
                    if (v.Edge2.Visited)
                    {
                        set.Remove(v.Edge2);
                    }
                }
                else
                {
                    var distance = Vector2.DistanceSquared(center.Position, v.Position + (incidentVertex2.Position - v.Position) / 2.0f);

                    if (distance > 0.0f)
                    {
                        v.Edge2.DistanceSquaredToCenter = distance;
                        v.Edge2.Visited = true;
                        set.Add(v.Edge2);
                    }
                }

            }

        }

        private class CompareVertexAngle : IComparer<Vertex>
        {
            public int Compare(Vertex x, Vertex y)
            {
                if (x.AngleToCenter < y.AngleToCenter)
                {
                    return -1;
                }
                if (x.AngleToCenter > y.AngleToCenter)
                {
                    return 1;
                }
                if (x.DistanceToCenter < y.DistanceToCenter)
                {
                    return -1;
                }
                if (x.DistanceToCenter > y.DistanceToCenter)
                {
                    return 1;
                }
                return 0;
            }
        }

        private class CompareEdgeDistance : IComparer<Edge>
        {
            public int Compare(Edge x, Edge y)
            {
                if (x.DistanceSquaredToCenter < y.DistanceSquaredToCenter)
                {
                    return -1;
                }
                if (x.DistanceSquaredToCenter > y.DistanceSquaredToCenter)
                {
                    return 1;
                }
                return 0;
            }
        }

        private class CompareEdgeVertices : IEqualityComparer<Edge>
        {
            public bool Equals(Edge x, Edge y)
            {
                return (x.V1 == y.V1 && x.V2 == y.V2) || (x.V1 == y.V2 && x.V2 == y.V1);
            }

            public int GetHashCode(Edge obj)
            {
                return obj.V1.GetHashCode() ^ obj.V2.GetHashCode();
            }
        }

        private class CompareVertices : IEqualityComparer<Vertex>
        {
            public bool Equals(Vertex x, Vertex y)
            {
                return (x.Position == y.Position);
            }

            public int GetHashCode(Vertex obj)
            {
                return obj.Position.GetHashCode();
            }
        }

        private int CheckVectorSide(Vector2 v1, Vector2 v2, float x, float y)
        {
            return Math.Sign((v2.X - v1.X) * (y - v1.Y) - (v2.Y - v1.Y) * (x - v1.X));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mDrawable", mDrawable);
            info.AddValue("mEdges", mEdges);
            info.AddValue("mVertices", mVertices);
            info.AddValue("mObstacleEdges", mObstacleEdges);
            /*
                public static List<Edge> mDrawable = new List<Edge>();

                public List<Edge> mEdges;
                public List<Vertex> mVertices;

                private List<Edge> mObstacleEdges;
             */
        }

        public VisibilityGraph(SerializationInfo info, StreamingContext context)
        {
            mDrawable = (List<Edge>)info.GetValue("mDrawable", typeof(List<Edge>));
            mEdges = (List<Edge>)info.GetValue("mEdges", typeof(List<Edge>));
            mVertices = (List<Vertex>)info.GetValue("mVertices", typeof(List<Vertex>));
            mObstacleEdges = (List<Edge>)info.GetValue("mObstacleEdges", typeof(List<Edge>));
        }
        public VisibilityGraph() { }
    }

}
