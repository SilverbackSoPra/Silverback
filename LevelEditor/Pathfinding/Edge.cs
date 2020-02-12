using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Pathfinding
{
    [Serializable()]
    public class Edge: ISerializable
    {

        public Vertex V1 { get; }
        public Vertex V2 { get; }

        public float Weight { get; set; }

        public float DistanceSquaredToCenter { get; set; }

        public bool Visited { get; set; }

        public Edge(Vertex v1, Vertex v2, float weight)
        {

            V1 = v1;
            V2 = v2;

            Weight = weight;
            Visited = false;

        }

        public bool Intersects(Edge edge, ref Vector2 intersection)
        {

            var line1P1 = V1.Position;
            var line1P2 = V2.Position;

            var line2P1 = edge.V1.Position;
            var line2P2 = edge.V2.Position;

            var seg1 = line1P2 - line1P1;
            var seg2 = line2P2 - line2P1;

            var determinant = -seg2.X * seg1.Y + seg1.X * seg2.Y;

            // If the lines are parallel or collinear we assume there is now intersection
            // This also solves division by zero
            if (determinant == 0.0f)
            {
                return false;
            }

            var s = (-seg1.Y * (line1P1.X - line2P1.X) + seg1.X * (line1P1.Y - line2P1.Y)) / determinant;
            var t = (seg2.X * (line1P1.Y - line2P1.Y) - seg2.Y * (line1P1.X - line2P1.X)) / determinant;

            if(s > 0.0f && s < 1.0f && t > 0.0f && t < 1.0f)
            {

                intersection = line1P1 + t * seg1;
                if (intersection != edge.V1.Position && intersection != edge.V2.Position)
                {
                    return true;
                }

            }

            return false;

        }

        public Vertex GetOtherVertex(Vertex vertex)
        {
            return vertex == V1 ? V2 : V1;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("V1", V1);
            info.AddValue("V2", V2);
            info.AddValue("Weight", Weight);
            info.AddValue("DistanceSquaredToCenter", DistanceSquaredToCenter);
            info.AddValue("Visited", Visited);
        }

        public Edge(SerializationInfo info, StreamingContext context)
        {
            V1 = (Vertex)info.GetValue("V1", typeof(Vertex));
            V2 = (Vertex)info.GetValue("V2", typeof(Vertex));
            Weight = (float)info.GetValue("Weight", typeof(float));
            DistanceSquaredToCenter = (float)info.GetValue("DistanceSquaredToCenter", typeof(float));
            Visited = (bool)info.GetValue("Visited", typeof(bool));
        }
        public Edge() { }
    }
}
