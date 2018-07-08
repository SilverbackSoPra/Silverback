using LevelEditor.Engine.Animation;
using LevelEditor.Sound;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using LevelEditor.Collision;

namespace LevelEditor.Engine.Mesh
{
    /// <summary>
    /// An actor represents an instance of a mesh in a scene.
    /// </summary>
    internal sealed class Actor
    {

        public readonly List<AudioSource> mAudioSources;

        // ReSharper disable InconsistentNaming
        public IActor IActor { get; private set; }

        public bool Collision { get; set; }

        public bool QuadTree { get; set; }

        public bool Intersects { get; set; }

        public Matrix ModelMatrix
        {
            get { return mModelMatrix; }
            set
            {

                Matrix matrix;

                if (mMesh.mMeshData.mRootTransformation != Matrix.Identity)
                {
                    matrix = mMesh.mMeshData.mRootTransformation * value;
                }
                else
                {
                    matrix = value;
                }

                mModelMatrix = matrix;
                mBoundingRectangle.Translate(matrix);

            }
        }

        public Vector3 Color { get; set; }

        public readonly Mesh mMesh;

        public readonly CollisionRectangle mBoundingRectangle;

        public readonly Animator mAnimator;

        public bool mRender;
        public bool mCastShadow;

        private Matrix mModelMatrix;

        /// <summary>
        /// Constructs an <see cref="Actor"/>.
        /// </summary>
        /// <param name="mesh">The <see cref="Mesh"/> which the actor represents in the scene</param>
        public Actor(Mesh mesh)
        {

            mMesh = mesh;

            mAudioSources = new List<AudioSource>();
            mBoundingRectangle = new CollisionRectangle(mesh.mMeshData.mBoundingRectangle);

            mModelMatrix = mMesh.mMeshData.mRootTransformation * Matrix.Identity;

            mAnimator = new Animator(mesh);

            Color = new Vector3(1.0f);
            
            // Actor should be rendered
            mRender = true;
            mCastShadow = true;

            IActor = null;

        }

        /// <summary>
        /// Constructs an <see cref="Actor"/>.
        /// </summary>
        /// <param name="mesh">The <see cref="Mesh"/> which the actor represents in the scene</param>
        public Actor(Mesh mesh, IActor actor)
        {

            mMesh = mesh;

            mAudioSources = new List<AudioSource>();
            mBoundingRectangle = new CollisionRectangle(mesh.mMeshData.mBoundingRectangle);

            ModelMatrix = Matrix.Identity;

            mAnimator = new Animator(mesh);

            Color = new Vector3(1.0f);

            // Actor should be rendered
            mRender = true;
            mCastShadow = true;

            IActor = actor;

        }

    }

}
