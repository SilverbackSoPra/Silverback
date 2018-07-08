using System;
using Microsoft.Xna.Framework;

namespace LevelEditor.Engine.Helper
{
    internal static class MathExtension
    {

        /// <summary>
        /// Calculates the mixed vector like x + factor * (y - x)
        /// </summary>
        /// <param name="x">The first vector</param>
        /// <param name="y">the second vector</param>
        /// <param name="factor">The mix value</param>
        /// <returns>The mixed vector</returns>
        public static Vector2 Mix(Vector2 x, Vector2 y, float factor)
        {
            return x + factor * (y - x);
        }

        /// <summary>
        /// Calculates the mixed vector like x + factor * (y - x)
        /// </summary>
        /// <param name="x">The first vector</param>
        /// <param name="y">the second vector</param>
        /// <param name="factor">The mix value</param>
        /// <returns>The mixed vector</returns>
        public static Vector3 Mix(Vector3 x, Vector3 y, float factor)
        {
            return x + factor * (y - x);
        }

        /// <summary>
        /// Clamps the variable to the range of [min, max]
        /// </summary>
        /// <param name="value">The variable to be clamped</param>
        /// <param name="min">The min of the range</param>
        /// <param name="max">The max of the range</param>
        /// <returns>Clamped value in range of [min, max]</returns>
        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary>
        /// Finds the maximum component of the vector
        /// </summary>
        /// <param name="vector">The vector</param>
        /// <returns>The value of the maximum component</returns>
        public static float Max(Vector3 vector)
        {
            return Math.Max(Math.Max(vector.X, vector.Y), Math.Max(vector.Y, vector.Z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 Abs(Vector3 vector)
        {
            return new Vector3(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
        }


    }
}
