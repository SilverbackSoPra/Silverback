using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

// ReSharper disable InconsistentNaming

namespace LevelEditor.Engine.Helper
{

    /// <summary>
    /// A new type of Vertex structure which is used to just transfer texture coordinates.
    /// </summary>
    internal struct VertexTexture
    {
        private Vector2 TextureCoordinate;

        internal static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        internal VertexTexture(float x, float y)
        {
            TextureCoordinate = new Vector2(x, y);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    internal struct VertexPositionTextureSkinned
    {

        public Vector3 Position;
        public Vector2 TextureCoordinate;
        public Vector4 BoneWeights;
        public Byte4 BoneIndex;

        internal static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 5, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(sizeof(float) * 9, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0)
        );

    }

    internal struct GrassInstancing
    {

        public Vector3 Position;

        internal static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
        );

    }

}
