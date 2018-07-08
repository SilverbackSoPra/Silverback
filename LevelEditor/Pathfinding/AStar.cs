using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using Assimp;
using Microsoft.Xna.Framework;

namespace LevelEditor.Pathfinding
{
    public static class AStar
    {
        public static Vertex Search(Vertex start, Vertex end, IEnumerable<Vertex> vertices, IEnumerable<Edge> edges,
            Func<Vertex, Edge, Vertex> extractOtherVertex)
        {
            var open = new SortedSet<Vertex>();
            
            var closed = new HashSet<Vertex>();

            var currentNode = start;
            currentNode.mDestination = end;
            currentNode.mCost = 0.0f;
            currentNode.mHeuristicCost = Vector2.Distance(start.Position, end.Position);
            var firstNode = currentNode;


            open.Add(firstNode);
            while (open.Count > 0)
            {
                currentNode = open.Min;
                if (currentNode.Equals(end))
                {
                    break;
                }

                open.Remove(currentNode);

                closed.Add(currentNode);

                // Get all Nodes containing currentNode.mVertex
                var tmpEdges = FindEdges(currentNode, edges);
                var successors = tmpEdges.Select(edge => extractOtherVertex(currentNode, edge)).ToList();

                // Iterate over all successors of currentNode.mVertex
                foreach (var successor in successors)
                {
                    if (closed.Contains(successor))
                    {
                        continue;
                    }

                    successor.mDestination = end;

                    var cost = currentNode.mCost + Vector2.Distance(currentNode.Position, successor.Position);

                    
                    if (open.Contains(successor) && cost >= successor.mCost)
                    {
                        continue;
                    }
                    
                    successor.mPreviousVertex = currentNode;
                    currentNode.mNextVertex = successor;
                    successor.mCost = cost;

                    // Calculate the heurisic cost from start over nextVertex to end
                    var hCost = successor.mCost + Vector2.Distance(successor.Position, end.Position);

                    if (open.Contains(successor))
                    {
                        // Adjust the cost value
                        open.Remove(successor);
                        successor.mHeuristicCost = hCost;
                        open.Add(successor);
                        continue;
                    }
                    open.Add(successor);
                }

            }
            
            return firstNode;
        }

    
        private static IEnumerable<Edge> FindEdges(Vertex vertex, IEnumerable<Edge> edges)
        {
            Func<Vertex, Edge, bool> isVertexInEdge = (v, edge) =>
            {
                // For debuging splitted into multiple lines
                var a = edge.V1 == v;
                var b = edge.V2 == v;
                return a || b;
            };

            return edges.Where(edge => isVertexInEdge(vertex, edge));
        }
    }
    

}
