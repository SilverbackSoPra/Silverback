using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.Engine.Mesh
{
    internal sealed class Material
    {

        public Texture2D mDiffuseMap;

        public Vector3 mDiffuseColor;

        public float mSpecularHardness;
        public float mSpecularIntensitiy;

        public Material()
        {

            mDiffuseMap = null;
            mDiffuseColor = new Vector3(1.0f);

            mSpecularHardness = 0.0f;
            mSpecularIntensitiy = 0.0f;

        }

    }
}
