using LevelEditor.Engine.Helper;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LevelEditor.Engine.Mesh
{

    /// <summary>
    /// Represents a mesh or model and contains all necessary buffers for the GPU.
    /// </summary>
    internal sealed class Mesh : IDisposable
    {

        public string Path { get; set; }

        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }

        public readonly MeshData mMeshData;

        private bool mDisposed = false;

        /// <summary>
        /// Constructs a <see cref="Mesh"/>.
        /// </summary>
        /// <param name="model">A textured model which was loaded with the content pipeline of MonoGame.</param>
        public Mesh(Model model)
        {

            // How to use animations: 
            mMeshData = new MeshData();

            if (model.Meshes.Count != 1)
            {
                throw new EngineInvalidParameterException("Model has more than one mesh");
            }
            else
            {
                if (model.Meshes[0].MeshParts.Count != 1)
                {
                    throw new EngineInvalidParameterException("Model probably has more than one material");
                }
                else
                {

                    mMeshData.mBoundingSphere = model.Meshes[0].BoundingSphere;

                    var material = new Material();
                    var subData = new MeshData.SubData();

                    material.mDiffuseMap = ((BasicEffect)model.Meshes[0].MeshParts[0].Effect).Texture;

                    subData.mIndicesOffset = 0;
                    subData.mMaterialIndex = 0;
                    subData.mNumPrimitives = model.Meshes[0].MeshParts[0].PrimitiveCount;

                    mMeshData.mSubDatas.Add(subData);
                    mMeshData.mMaterials.Add(material);

                    mMeshData.mTotalNumPrimitives = subData.mNumPrimitives;

                    VertexBuffer = model.Meshes[0].MeshParts[0].VertexBuffer;
                    IndexBuffer = model.Meshes[0].MeshParts[0].IndexBuffer;

                }
                
            }

        }

        /// <summary>
        /// Constructs a <see cref="Mesh"/>.
        /// </summary>
        /// <param name="device">The graphics device which should already be initialized.</param>
        /// <param name="data">The data of the mesh. All the data should've already been loaded.</param>
        public Mesh(GraphicsDevice device, MeshData data)
        {

            if (data.mBoundingSphere.Radius <= 0.0f)
            {
                throw new EngineInvalidParameterException("Radius should be larger than zero");
            }
            else
            {
                if ((data.mVertices == null && data.mVerticesExt == null) || data.mIndices == null)
                {
                    throw new EngineInvalidParameterException("Invalid vertices or indices count");
                }
                else
                {

                    mMeshData = data;

                    // We have to keep in mind that we need to differantiate between skinned and unskinned meshes
                    if (data.mIsSkinned)
                    {
                        VertexBuffer = new VertexBuffer(device,
                            VertexPositionTextureSkinned.VertexDeclaration,
                            mMeshData.mVerticesExt.Length,
                            BufferUsage.WriteOnly);
                        VertexBuffer.SetData(mMeshData.mVerticesExt);
                    }
                    else
                    {
                        VertexBuffer = new VertexBuffer(device,
                            VertexPositionTexture.VertexDeclaration,
                            mMeshData.mVertices.Length,
                            BufferUsage.WriteOnly);
                        VertexBuffer.SetData(mMeshData.mVertices);
                    }

                    IndexBuffer = new IndexBuffer(device,
                        typeof(int),
                        mMeshData.mIndices.Length,
                        BufferUsage.WriteOnly);
                    
                    IndexBuffer.SetData(mMeshData.mIndices);

                }
                    
            }
        }

        /// <summary>
        /// Updates the data on the GPU based on the changed <see cref="MeshData"/>
        /// </summary>
        public void UpdateData()
        {

            VertexBuffer.SetData(mMeshData.mVertices);
            IndexBuffer.SetData(mMeshData.mIndices);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animation"></param>
        public void Add(Animation.Animation animation)
        {



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animation"></param>
        public void Remove(Animation.Animation animation)
        {



        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {

            if (mDisposed)
            {
                return;
            }

            if (disposing)
            {
                VertexBuffer.Dispose();
                IndexBuffer.Dispose();
            }

            mDisposed = true;

        }

    }

}
