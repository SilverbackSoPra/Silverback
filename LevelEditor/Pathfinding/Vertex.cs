using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using LevelEditor.Collision;

namespace LevelEditor.Pathfinding
{
    [Serializable()]
    public class Vertex: ISerializable
    {

        public static int mCounter = 0;

        public int mVersion;

        [XmlIgnore]
        public Vertex mNextVertex;
        [XmlIgnore]
        public Vertex mPreviousVertex;
        public float mCost;
        public float mHeuristicCost;
        public int mIterations = 0;

        public Vector2 Position { get; }

        public bool Connectable { get; set; }

        public CollisionRectangle ParentRectangle { get; }

        public float AngleToCenter { get; set; }
        public float DistanceToCenter { get; set; }

        public Edge Edge1 { get; set; }
        public Edge Edge2 { get; set; }

        [XmlIgnore]
        public HashSet<Vertex> mAdjacentVertices;

        public Vertex(Vector2 position, CollisionRectangle parent)
        {

            mVersion = ++mCounter;
            Position = position;
            ParentRectangle = parent;
            Connectable = true;
            mCost = float.MaxValue;
            mHeuristicCost = float.MaxValue;
            mAdjacentVertices = new HashSet<Vertex>();
        }

        public float Angle(Vertex vertex)
        {

            return (float)Math.Atan2(vertex.Position.X - Position.X, vertex.Position.Y - Position.Y);

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mCounter", mCounter);
            info.AddValue("mVersion", mVersion);
            info.AddValue("mNextVertex", mNextVertex);
            info.AddValue("mPreviousVertex", mPreviousVertex);
            info.AddValue("mCost", mCost);
            info.AddValue("mHeuristicCost", mHeuristicCost);
            info.AddValue("Position", Position);
            info.AddValue("Connectable", Connectable);
            info.AddValue("ParentRectangle", ParentRectangle);
            info.AddValue("AngleToCenter", AngleToCenter);
            info.AddValue("Edge1", Edge1);
            info.AddValue("Edge2", Edge2);
        }

        public Vertex(SerializationInfo info, StreamingContext context)
        {
            mCounter = (int)info.GetValue("mCounter", typeof(int));
            mVersion = (int)info.GetValue("mVersion", typeof(int));
            mNextVertex = (Vertex)info.GetValue("mNextVertex", typeof(Vertex));
            mPreviousVertex = (Vertex)info.GetValue("mPreviousVertex", typeof(Vertex));
            mCost = (float)info.GetValue("mCost", typeof(float));
            mHeuristicCost = (float)info.GetValue("mHeuristicCost", typeof(float));
            Position = (Vector2)info.GetValue("Position", typeof(Vector2));
            Connectable = (bool)info.GetValue("Connectable", typeof(bool));
            ParentRectangle = (CollisionRectangle)info.GetValue("ParentRectangle", typeof(CollisionRectangle));
            AngleToCenter = (float)info.GetValue("AngleToCenter", typeof(float));
            Edge1 = (Edge)info.GetValue("Edge1", typeof(Edge));
            Edge2 = (Edge)info.GetValue("Edge2", typeof(Edge));
        }
        public Vertex() { }
    }

}
