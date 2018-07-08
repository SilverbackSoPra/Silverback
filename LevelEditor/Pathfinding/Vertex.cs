using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using LevelEditor.Collision;

namespace LevelEditor.Pathfinding
{
    public class Vertex: IComparable<Vertex>
    {

        public static int mCounter = 0;

        public int mVersion;

        public Vertex mNextVertex;
        public Vertex mPreviousVertex;
        public Vertex mDestination;
        public float mCost;
        public float mHeuristicCost;

        public Vector2 Position { get; }
        public CollisionRectangle ParentRectangle { get; }

        public float AngleToCenter { get; set; }

        public Edge Edge1 { get; set; }
        public Edge Edge2 { get; set; }

        public Vertex(Vector2 position, CollisionRectangle parent)
        {
            mVersion = ++mCounter;
            Position = position;
            ParentRectangle = parent;

        }

        public float Angle(Vertex vertex)
        {

            return (float)Math.Atan2(vertex.Position.X - Position.X, vertex.Position.Y - Position.Y);

        }
        public int CompareTo(Vertex other)
        {
            var thisDistance = Vector2.Distance(Position, mDestination.Position);
            var otherDistance = Vector2.Distance(other.Position, mDestination.Position);
            if (thisDistance < otherDistance)
            {
                return -1;
            }
            if (thisDistance > otherDistance)
            {
                return 1;
            }

            return 0;
        }
    }

}
