using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Collision;
using Microsoft.Xna.Framework;

namespace LevelEditor.Pathfinding
{
    class VisibilityGraph
    {

        public List<Edge> mEdges;
        public List<Vertex> mVertices;

        private List<Edge> mObstacleEdges;

        public VisibilityGraph(List<CollisionRectangle> staticCollisionRectangles, bool bruteForce)
        {

            // We should instead use arrays for better performance.
            // Especially because foreach loops are way slower when using lists
            mVertices = new List<Vertex>();

            mObstacleEdges = new List<Edge>();
            var edgesHashSet = new HashSet<Edge>(new CompareEdgeVertices());

            foreach (var rectangle in staticCollisionRectangles)
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

            }

            var vertices = mVertices.ToArray();

            foreach (var vertex in vertices)
            {

                if (!bruteForce)
                {
                    CalculateVisibleVertices(vertex, ref edgesHashSet);
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
            Vertex startVertex = new Vertex(new Vector2(start.X, start.Z), mVertices[0].ParentRectangle);
            Vertex destinationVertex = new Vertex(new Vector2(destination.X, destination.Z), mVertices[0].ParentRectangle);
            
            CalculateVisibleVertices(startVertex, ref edgeHashSet);
            CalculateVisibleVertices(destinationVertex, ref edgeHashSet);

            var directEdge = new Edge(startVertex, destinationVertex, 0.0f);

            startVertex.mDestination = destinationVertex;

            // If there exists a direct path, we dont have to use A*
            if (edgeHashSet.Contains(directEdge))
            {
                startVertex.mNextVertex = destinationVertex;
                startVertex.mNextVertex.mPreviousVertex = startVertex;
                return startVertex;
            }

            // Now combine the hash set with mEdges and calculate the path
            // Note: We shouldn't insert the hash set into mEdges. Instead we
            // create a new list from edgeHashSet and append mEdges to it
            var edgeList = edgeHashSet.ToList();
            edgeList.AddRange(mEdges);

            // Now calculate the path with A*
            startVertex = AStar.Search(startVertex,
                destinationVertex,
                mVertices,
                edgeList,
                (vertex, edge) =>
                {
                    if (edge.V1 == vertex)
                    {
                        return edge.V2;
                    }
                    if (edge.V2 == vertex)
                    {
                        return edge.V1;
                    }

                    return null;
                });



            return startVertex;

        }

        private void CalculateVisibleVertices(Vertex vertex, ref HashSet<Edge> edgesHashSet)
        {

            var center = vertex;

            foreach (var edge in mObstacleEdges)
            {
                edge.Visited = false;
            }

            // Sort the vertices in angular order
            // Calculate the angles to the center vertex
            foreach (var v in mVertices)
            {
                v.AngleToCenter = center.Angle(v);
            }

            mVertices.Sort(new CompareVertexAngle());

            // Create the scanline with the vertex that has the smallest angle
            var p = new Vertex(new Vector2(mVertices[0].Position.X, mVertices[0].Position.Y), center.ParentRectangle);
            var scanLine = new Edge(center, p, 0.0f);

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
            foreach (var v in mVertices)
            {

                var edge = new Edge(center, v, 0.0f);
                bool locked = false;

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
                            edgesHashSet.Add(edge);
                        }
                    }
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

        private int CheckVectorSide(Vector2 v1, Vector2 v2, float x, float y)
        {
            return Math.Sign((v2.X - v1.X) * (y - v1.Y) - (v2.Y - v1.Y) * (x - v1.X));
        }

    }

}
