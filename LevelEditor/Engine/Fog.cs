using Microsoft.Xna.Framework;

namespace LevelEditor.Engine
{
    internal sealed class Fog
    {

        public Vector3 mColor;
        public float mDistance;

        public Fog()
        {

            mColor = new Vector3(1.0f);
            mDistance = 50.0f;

        }

    }
}
