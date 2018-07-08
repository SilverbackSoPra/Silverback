using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using LevelEditor.Collision;
using LevelEditor.Engine.Animation;
using LevelEditor.Engine.Helper;

namespace LevelEditor.Engine.Mesh
{


    /// <summary>
    /// Contains all the data of a mesh like textures or vertices.
    /// </summary>
    internal sealed class MeshData
    {

        public struct SubData
        {

            public int mIndicesOffset;
            public int mNumPrimitives;
            public int mMaterialIndex;

        }

        // We use the normal vertices when the model isn't skinned
        public VertexPositionTexture[] mVertices;
        // We use the extended vertices if the model is skinned
        public VertexPositionTextureSkinned[] mVerticesExt;

        public int[] mIndices;

        public PrimitiveType mPrimitiveType;
        public int mTotalNumPrimitives;

        public readonly List<Material> mMaterials;
        public readonly List<SubData> mSubDatas;

        public readonly List<Animation.Animation> mAnimations;
        public Bone[] mBones;
        public bool mIsSkinned;

        public BoundingSphere mBoundingSphere;
        public CollisionRectangle mBoundingRectangle;

        public Matrix mRootTransformation;
        public bool mIsTerrain;

        /// <summary>
        /// Constructs a <see cref="MeshData"/>
        /// </summary>
        public MeshData()
        {

            mVertices = null;
            mVerticesExt = null;
            mIndices = null;
            mBones = null;

            mMaterials = new List<Material>();
            mSubDatas = new List<SubData>();
            mAnimations = new List<Animation.Animation>();
            mIsSkinned = false;

            mPrimitiveType = PrimitiveType.TriangleList;
            mTotalNumPrimitives = 0;

            mBoundingSphere = new BoundingSphere(new Vector3(0.0f), 0.0f);
            mBoundingRectangle = new CollisionRectangle(Vector2.Zero, Vector2.Zero, Vector2.Zero);

            mRootTransformation = Matrix.Identity;
            mIsTerrain = false;

        }

    }

}
