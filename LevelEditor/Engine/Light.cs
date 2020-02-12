using Microsoft.Xna.Framework;

namespace LevelEditor.Engine
{

    /// <summary>
    /// Represents a light which can be added to a scene.
    /// </summary>
    public sealed class Light
    {

        public Vector3 mLocation;
        public Vector3 mColor;

        public readonly Shadow mShadow;

        public float mAmbient;

        /// <summary>
        /// Constructs a <see cref="Light"/>.
        /// </summary>
        /// <param name="location">The location of the light.</param>
        /// <param name="color">The color of the light in the range of [0.0-1.0] per component (RGB).</param>
        /// <param name="ambient">The minimum brightness of the darkest regions.</param>
        public Light(Vector3 location = default(Vector3), 
            Vector3 color = default(Vector3),
            float ambient = 0.2f)
        {

            mLocation = location == default(Vector3) ? new Vector3(0.0f, 10000.0f, 0.0f) : location;
            mColor = color == default(Vector3) ? new Vector3(1.0f) : color; 

            mAmbient = ambient;

            mShadow = new Shadow(this);

        }

    }
}
