using System;
using Microsoft.Xna.Framework;

namespace LevelEditor.Collision
{
    public class CollisionRectangle
    {
        private Vector2 V1 { get; set; }
        private Vector2 V2 { get; set; }
        private Vector2 V3 { get; set; }
        
        public Vector2 V1Transformed { get; private set; }
        public Vector2 V2Transformed { get; private set; }
        public Vector2 V3Transformed { get; private set; }
        public Vector2 V4Transformed { get; private set; }

        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        private Vector2 V1V2 { get; set; }
        private Vector2 V1V3 { get; set; }
        private float DotV1V2 { get; set; }
        private float DotV1V3 { get; set; }

        private bool mHasMoved;
        private Rectangle mAxisAlignedRectangle;


        public CollisionRectangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            Translate(Matrix.Identity);
        }

        public CollisionRectangle(CollisionRectangle rectangle)
        {
            V1 = rectangle.V1;
            V2 = rectangle.V2;
            V3 = rectangle.V3;
            Translate(Matrix.Identity);
            mHasMoved = true;
        }

        public bool IntersectsRectangle(CollisionRectangle r, bool checkOther)
        {
            
            if (PointInside(r.V1Transformed, V1V2, V1V3, DotV1V2, DotV1V3))
            {
                return true;
            }
            if (PointInside(r.V2Transformed, V1V2, V1V3, DotV1V2, DotV1V3))
            {
                return true;
            }
            if (PointInside(r.V3Transformed, V1V2, V1V3, DotV1V2, DotV1V3))
            {
                return true;
            }
            if (PointInside(r.V4Transformed, V1V2, V1V3, DotV1V2, DotV1V3))
            {
                return true;
            }

            if (checkOther)
            {
                return r.IntersectsRectangle(this, false);
            }

            return false;

        }
    
        public bool IntersectsLine(Vector2 vec1, Vector2 vec2)
        {

            return LineLineIntersection(vec1, vec2, V1Transformed, V4Transformed) ||
                LineLineIntersection(vec1, vec2, V2Transformed, V3Transformed);

        }

        public void Translate(Matrix matrix)
        {

            var v1 = new Vector3(V1.X, 0.0f, V1.Y);
            var v2 = new Vector3(V2.X, 0.0f, V2.Y);
            var v3 = new Vector3(V3.X, 0.0f, V3.Y);

            var v1T = Vector3.Transform(v1, matrix);
            var v2T = Vector3.Transform(v2, matrix);
            var v3T = Vector3.Transform(v3, matrix);

            V1Transformed = new Vector2(v1T.X, v1T.Z);
            V2Transformed = new Vector2(v2T.X, v2T.Z);
            V3Transformed = new Vector2(v3T.X, v3T.Z);

            V4Transformed = V2Transformed + V3Transformed - V1Transformed;

            V1V2 = V2Transformed - V1Transformed;
            V1V3 = V3Transformed - V1Transformed;

            DotV1V2 = Vector2.Dot(V1V2, V1V2);
            DotV1V3 = Vector2.Dot(V1V3, V1V3);

            CalculateMinMax();
            mHasMoved = true;

        }

        private bool PointInside(Vector2 p, Vector2 v1V2, Vector2 v1V3, float dotV1V2, float dotV1V3)
        {

            var v1P = p - V1Transformed;
            var dotV1V2V1P = v1V2.X * v1P.X + v1V2.Y * v1P.Y;
            var dotV2V3V2P = v1V3.X * v1P.X + v1V3.Y * v1P.Y;

            return 0.0f <= dotV1V2V1P && dotV1V2V1P <= dotV1V2 && 0.0f <= dotV2V3V2P && dotV2V3V2P <= dotV1V3;

        }

        private bool LineLineIntersection(Vector2 line1P1, Vector2 line1P2, Vector2 line2P1, Vector2 line2P2)
        {

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

            return (s >= 0.0f && s <= 1.0f && t >= 0.0f && t <= 1.0f);

        }

        public CollisionRectangle Scale(float scale)
        { 
            var vc = V1Transformed + (V2Transformed - V1Transformed) / 2.0f + (V3Transformed - V1Transformed) / 2.0f;
            var v1 = vc + (V1Transformed - vc) * scale;
            var v2 = vc + (V2Transformed - vc) * scale;
            var v3 = vc + (V3Transformed - vc) * scale;

            return new CollisionRectangle(v1, v2, v3);
 
        }

        public Rectangle GetAxisAlignedRectangle(int scale)
        {
            if (mHasMoved)
            {
                mAxisAlignedRectangle = new Rectangle
                {
                    X = (int)Min.X,
                    Y = (int)Min.Y,
                    Width = (int)Math.Ceiling(Max.X - Min.X),
                    Height = (int)Math.Ceiling(Max.Y - Min.Y)
                };

                mAxisAlignedRectangle.X = mAxisAlignedRectangle.X - mAxisAlignedRectangle.Width * scale;
                mAxisAlignedRectangle.Y = mAxisAlignedRectangle.Y - mAxisAlignedRectangle.Height * scale;
                mAxisAlignedRectangle.Width = mAxisAlignedRectangle.Width * 2 * scale;
                mAxisAlignedRectangle.Height = mAxisAlignedRectangle.Height * 2 * scale;
                mHasMoved = false;
            }

            return mAxisAlignedRectangle;
        }

        private void CalculateMinMax()
        {

            var min = V1Transformed;
            var max = V1Transformed;

            max.X = V2Transformed.X > max.X ? V2Transformed.X : max.X;
            max.Y = V2Transformed.Y > max.Y ? V2Transformed.Y : max.Y;

            min.X = V2Transformed.X < min.X ? V2Transformed.X : min.X;
            min.Y = V2Transformed.Y < min.Y ? V2Transformed.Y : min.Y;

            max.X = V3Transformed.X > max.X ? V3Transformed.X : max.X;
            max.Y = V3Transformed.Y > max.Y ? V3Transformed.Y : max.Y;

            min.X = V3Transformed.X < min.X ? V3Transformed.X : min.X;
            min.Y = V3Transformed.Y < min.Y ? V3Transformed.Y : min.Y;

            max.X = V4Transformed.X > max.X ? V4Transformed.X : max.X;
            max.Y = V4Transformed.Y > max.Y ? V4Transformed.Y : max.Y;

            min.X = V4Transformed.X < min.X ? V4Transformed.X : min.X;
            min.Y = V4Transformed.Y < min.Y ? V4Transformed.Y : min.Y;

            Min = min;
            Max = max;

        }

    }

}
