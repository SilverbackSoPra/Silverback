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

        private static CompareVertexCost mCompareVertexCost = new CompareVertexCost();

        public static Vertex Search(Vertex start, Vertex end, IEnumerable<Vertex> vertices)
        {

            var open = new SortedSet<Vertex>(new CompareVertexCost());

            var closed = new HashSet<Vertex>();

            foreach (var vertex in vertices)
            {
                vertex.mHeuristicCost = float.MaxValue;
                vertex.mCost = float.MaxValue;
            }

            var currentNode = start;
            currentNode.mCost = 0.0f;
            currentNode.mHeuristicCost = Vector2.DistanceSquared(start.Position, end.Position);
            var firstNode = currentNode;

            open.Add(firstNode);

            while (open.Count > 0)
            {

                currentNode = open.Min;

                if (currentNode.Equals(end))
                {
                    break;
                }

                open.Remove(open.Min);
                closed.Add(currentNode);

                // Get all Nodes containing currentNode.mVertex
                var neighbours = currentNode.mAdjacentVertices;

                // Iterate over all successors of currentNode.mVertex
                foreach (var neighbour in neighbours)
                {

                    if (closed.Contains(neighbour))
                    {
                        continue;
                    }

                    var cost = currentNode.mCost + Vector2.DistanceSquared(currentNode.Position, neighbour.Position);
                    
                    if (!open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                    }
                    else if (cost >= neighbour.mCost)
                    {
                        continue;
                    }

                    open.Remove(neighbour);
                    neighbour.mPreviousVertex = currentNode;
                    neighbour.mCost = cost;
                    neighbour.mHeuristicCost = cost + Vector2.DistanceSquared(neighbour.Position, end.Position);
                    open.Add(neighbour);
                    
                }

            }

            while (currentNode.mPreviousVertex != null)
            {
                currentNode.mPreviousVertex.mNextVertex = currentNode;
                currentNode = currentNode.mPreviousVertex;
            }
            
            return firstNode;
        }

        private class CompareVertexCost : IComparer<Vertex>
        {
            public int Compare(Vertex x, Vertex y)
            {
                if (x.mHeuristicCost < y.mHeuristicCost)
                {
                    return -1;
                }
                if (x.mHeuristicCost > y.mHeuristicCost)
                {
                    return 1;
                }
                return 0;
            }
        }

    }
    

}
