
using System;
using System.Collections.Generic;
using System.IO;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework.Graphics;
using Material = LevelEditor.Engine.Mesh.Material;
using Assimp;
using Assimp.Configs;
using LevelEditor.Collision;
using LevelEditor.Engine.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Bone = LevelEditor.Engine.Animation.Bone;

namespace LevelEditor.Engine.Loader
{

    /// <summary>
    /// Is able to load meshes and animations.
    /// </summary>
    internal sealed class ModelLoader
    {

        private const float BoundingRectangleBias = 1.0f;
        private readonly GraphicsDevice mGraphicsDevice;

        /// <summary>
        /// Constructs a <see cref="ModelLoader"/>
        /// </summary>
        /// <param name="device">The graphics device which should already be initialized.</param>
        public ModelLoader(GraphicsDevice device)
        {

            mGraphicsDevice = device;

        }

        /// <summary>
        /// Loads a mesh from a file
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <returns>A mesh which contains the data of the file</returns>
        public Mesh.Mesh LoadMesh(string filename)
        {

            var vertexCount = 0;
            var indexCount = 0;
            var bonesCount = 0;

            var directory = Path.GetDirectoryName(filename);
            directory += "/";

            var meshData = new MeshData();
            var importer = new AssimpContext();

            var boneConfig = new MaxBoneCountConfig(4);

            importer.SetConfig(boneConfig);

            var scene = importer.ImportFile(filename,
                PostProcessSteps.ImproveCacheLocality | 
                PostProcessSteps.JoinIdenticalVertices |
                PostProcessSteps.LimitBoneWeights | 
                PostProcessSteps.Triangulate | 
                PostProcessSteps.OptimizeMeshes | 
                PostProcessSteps.RemoveRedundantMaterials |
                PostProcessSteps.MakeLeftHanded |
                PostProcessSteps.OptimizeGraph);

            // We want to sort all the meshes to their corresponding materials
            var meshSorted = new List<Assimp.Mesh>[scene.MaterialCount];

            for (var i = 0; i < scene.MaterialCount; i++)
            {
                meshSorted[i] = new List<Assimp.Mesh>();
            }

            foreach (var mesh in scene.Meshes)
            {

                var index = mesh.MaterialIndex;

                meshSorted[index].Add(mesh);

                vertexCount += mesh.VertexCount;
                indexCount += (mesh.FaceCount * 3);
                bonesCount += mesh.BoneCount;

            }

            // Load bones and animations
            if (scene.HasAnimations || bonesCount > 0)
            {

                meshData.mIsSkinned = true;
                meshData.mBones = CreateBoneHierarchy(scene);

                foreach (var animation in scene.Animations)
                {
                    meshData.mAnimations.Add(new Animation.Animation(animation, meshData.mBones));
                }

                meshData.mVerticesExt = new VertexPositionTextureSkinned[vertexCount];

                // Set the skinning informatio to 0
                for (var i = 0; i < meshData.mVerticesExt.Length; i++)
                {
                    meshData.mVerticesExt[i].BoneWeights.X = 0.0f;
                    meshData.mVerticesExt[i].BoneWeights.Y = 0.0f;
                    meshData.mVerticesExt[i].BoneWeights.Z = 0.0f;
                    meshData.mVerticesExt[i].BoneWeights.W = 0.0f;
                    meshData.mVerticesExt[i].BoneIndex = new Byte4(0.0f, 0.0f, 0.0f, 0.0f);
                }

            }
            else
            {

                meshData.mVertices = new VertexPositionTexture[vertexCount];

            }

            meshData.mIndices = new int[indexCount];

            var usedFaces = 0;
            var usedVertices = 0;
            var loadedVertices = 0;

            for (var i = 0; i < scene.MaterialCount; i++)
            {

                var material = LoadMaterial(scene.Materials[i], directory);

                var subData = new MeshData.SubData
                {
                    mIndicesOffset = usedFaces * 3,
                    mMaterialIndex = i
                };


                foreach (var mesh in meshSorted[i])
                {

                    for (var j = 0; j < mesh.BoneCount; j++)
                    {

                        var boneIndex = Array.FindIndex(meshData.mBones, ele => ele.mName == mesh.Bones[j].Name);
                        var bone = meshData.mBones[boneIndex];
                        bone.mOffset = AssimpToXna(mesh.Bones[j].OffsetMatrix);

                        var bonePosition = new Vector4(0.0f);

                        for (var k = 0; k < mesh.Bones[j].VertexWeightCount; k++)
                        {

                            var weight = mesh.Bones[j].VertexWeights[k];
                            
                            bonePosition += new Vector4(mesh.Vertices[weight.VertexID].X * weight.Weight, mesh.Vertices[weight.VertexID].Y * weight.Weight, mesh.Vertices[weight.VertexID].Z * weight.Weight, weight.Weight);

                            var indices = meshData.mVerticesExt[usedVertices + weight.VertexID].BoneIndex.ToVector4();

                            if (!(meshData.mVerticesExt[usedVertices + weight.VertexID].BoneWeights.X > 0.0f))
                            {
                                meshData.mVerticesExt[usedVertices + weight.VertexID].BoneWeights.X = weight.Weight;
                                indices.X = boneIndex;
                            }
                            else if (!(meshData.mVerticesExt[usedVertices + weight.VertexID].BoneWeights.Y > 0.0f))
                            {
                                meshData.mVerticesExt[usedVertices + weight.VertexID].BoneWeights.Y = weight.Weight;
                                indices.Y = boneIndex;
                            }
                            else if (!(meshData.mVerticesExt[usedVertices + weight.VertexID].BoneWeights.Z > 0.0f))
                            {
                                meshData.mVerticesExt[usedVertices + weight.VertexID].BoneWeights.Z = weight.Weight;
                                indices.Z = boneIndex;
                            }
                            else if (!(meshData.mVerticesExt[usedVertices + weight.VertexID].BoneWeights.W > 0.0f))
                            {
                                meshData.mVerticesExt[usedVertices + weight.VertexID].BoneWeights.W = weight.Weight;
                                indices.W = boneIndex;
                            }

                            meshData.mVerticesExt[usedVertices + weight.VertexID].BoneIndex = new Byte4(indices);

                        }

                        bonePosition /= bonePosition.W;
                        bone.mPosition = new Vector3(bonePosition.X, bonePosition.Y, bonePosition.Z);

                    }

                    // Copy the vertices
                    for (var j = 0; j < mesh.VertexCount; j++)
                    {

                        float radius;

                        if (meshData.mVertices != null)
                        {
                            meshData.mVertices[usedVertices].Position.X = mesh.Vertices[j].X;
                            meshData.mVertices[usedVertices].Position.Y = mesh.Vertices[j].Y;
                            meshData.mVertices[usedVertices].Position.Z = mesh.Vertices[j].Z;

                            radius = meshData.mVertices[usedVertices].Position.Length();
                        }
                        else
                        {
                            meshData.mVerticesExt[usedVertices].Position.X = mesh.Vertices[j].X;
                            meshData.mVerticesExt[usedVertices].Position.Y = mesh.Vertices[j].Y;
                            meshData.mVerticesExt[usedVertices].Position.Z = mesh.Vertices[j].Z;

                            radius = meshData.mVerticesExt[usedVertices].Position.Length();
                        }

                        var vertex = meshData.mVertices?[usedVertices].Position ?? meshData.mVerticesExt[usedVertices].Position;

                        

                        meshData.mBoundingSphere.Radius = Math.Max(radius, meshData.mBoundingSphere.Radius);

                        if (mesh.UVComponentCount[0] != 0)
                        {
                            if (meshData.mVertices != null)
                            {
                                meshData.mVertices[usedVertices].TextureCoordinate.X =
                                    mesh.TextureCoordinateChannels[0][j].X;
                                meshData.mVertices[usedVertices].TextureCoordinate.Y =
                                    mesh.TextureCoordinateChannels[0][j].Y;
                            }
                            else
                            {
                                meshData.mVerticesExt[usedVertices].TextureCoordinate.X =
                                    mesh.TextureCoordinateChannels[0][j].X;
                                meshData.mVerticesExt[usedVertices].TextureCoordinate.Y =
                                    mesh.TextureCoordinateChannels[0][j].Y;
                            }
                        }

                        usedVertices++;

                    }

                    // Copy the indices
                    for (var j = 0; j < mesh.FaceCount; j++)
                    {

                        for (var k = 0; k < 3; k++)
                        {

                            meshData.mIndices[usedFaces * 3 + k] = mesh.Faces[j].Indices[k] + loadedVertices;

                        }

                        usedFaces++;

                    }

                    loadedVertices = usedVertices;

                }

                subData.mNumPrimitives = usedFaces - subData.mIndicesOffset / 3;
                meshData.mSubDatas.Add(subData);
                meshData.mMaterials.Add(material);

            }

            if (meshData.mIsSkinned)
            {
                for (var i = 0; i < meshData.mVerticesExt.Length; i++)
                {
                    var vertex = meshData.mVerticesExt[i].BoneWeights;

                    // Should fix bone weights which are zero
                    if (vertex.X + vertex.Y + vertex.Z + vertex.W == 10000.0f)
                    {
                        //meshData.mVerticesExt[i].BoneWeights = meshData.mVerticesExt[i - 1].BoneWeights;
                        //meshData.mVerticesExt[i].BoneIndex = meshData.mVerticesExt[i - 1].BoneIndex;
                    }
                    else {
                        meshData.mVerticesExt[i].BoneWeights *= (1.0f / (vertex.X + vertex.Y + vertex.Z +
                                                       vertex.W));
                    }

                }

            }


            meshData.mTotalNumPrimitives = usedFaces;
            meshData.mRootTransformation = AssimpToXna(scene.RootNode.Transform);

            meshData.mBoundingRectangle = CalculateBoundingRectangle(meshData);           

            return new Mesh.Mesh(mGraphicsDevice, meshData);

        }

        public void LoadAnimation(Mesh.Mesh mesh, string filename)
        {

            var importer = new AssimpContext();

            var boneConfig = new MaxBoneCountConfig(4);

            importer.SetConfig(boneConfig);

            var scene = importer.ImportFile(filename,
                PostProcessSteps.LimitBoneWeights |
                PostProcessSteps.MakeLeftHanded |
                PostProcessSteps.OptimizeGraph);

            if (scene.HasAnimations)
            {

                foreach (var animation in scene.Animations)
                {
                    mesh.mMeshData.mAnimations.Add(new Animation.Animation(animation, mesh.mMeshData.mBones));
                }

            }

        }

        private Material LoadMaterial(Assimp.Material assimpMaterial, string directory)
        {

            var material = new Material();

            if (assimpMaterial.HasTextureDiffuse)
            {
                var diffuseMapStream = new FileStream(directory + assimpMaterial.TextureDiffuse.FilePath, FileMode.Open);
                material.mDiffuseMap = Texture2D.FromStream(mGraphicsDevice, diffuseMapStream);
                diffuseMapStream.Close();
            }

            if (assimpMaterial.HasColorDiffuse)
            {
                var color = assimpMaterial.ColorDiffuse;
                material.mDiffuseColor = new Vector3(color.R, color.G, color.B);
            }

            if (assimpMaterial.HasColorSpecular)
            {
                var color = assimpMaterial.ColorSpecular;
                material.mSpecularIntensitiy = new Vector3(color.R, color.G, color.B).Length();
                material.mSpecularHardness = assimpMaterial.Shininess;
            }
            return material;

        }

        private static CollisionRectangle CalculateBoundingRectangle(MeshData meshData)
        {

            var min = Vector2.Zero;
            var max = Vector2.Zero;

            if (meshData.mIsSkinned)
            {

                // We need the matrices to calculate the actual position
                var matrices = new Matrix[meshData.mBones.Length];
                Bone.CalculateBoneBaseTransformation(meshData.mBones, ref matrices);

                foreach (var vertex in meshData.mVerticesExt)
                {
                    var index = vertex.BoneIndex.ToVector4();
                    var boneTransform = matrices[(int) index.X] * vertex.BoneWeights.X;
                    boneTransform += matrices[(int)index.Y] * vertex.BoneWeights.Y;
                    boneTransform += matrices[(int)index.Z] * vertex.BoneWeights.Z;
                    boneTransform += matrices[(int)index.W] * vertex.BoneWeights.W;

                    var location = Vector3.Transform(vertex.Position, boneTransform);

                    if (location.Y < BoundingRectangleBias && location.Y > 0.0f)
                    {

                        if (max == Vector2.Zero)
                        {
                            max = new Vector2(location.X, location.Z);
                        }

                        if (min == Vector2.Zero)
                        {
                            max = new Vector2(location.X, location.Z);
                        }

                        max.X = location.X > max.X ? location.X : max.X;
                        max.Y = location.Z > max.Y ? location.Z : max.Y;

                        min.X = location.X < min.X ? location.X : min.X;
                        min.Y = location.Z < min.Y ? location.Z : min.Y;

                    }

                }

            }
            else
            {

                foreach (var vertex in meshData.mVertices)
                {

                    var location = vertex.Position;

                    if (location.Y < BoundingRectangleBias && location.Y > 0.0f)
                    {

                        if (max == Vector2.Zero)
                        {
                            max = new Vector2(location.X, location.Z);
                        }

                        if (min == Vector2.Zero)
                        {
                            max = new Vector2(location.X, location.Z);
                        }

                        max.X = location.X > max.X ? location.X : max.X;
                        max.Y = location.Z > max.Y ? location.Z : max.Y;

                        min.X = location.X < min.X ? location.X : min.X;
                        min.Y = location.Z < min.Y ? location.Z : min.Y;

                    }

                }

            }

            return new CollisionRectangle(new Vector2(min.X, max.Y), min, max);

        }

        private static Bone[] CreateBoneHierarchy(Assimp.Scene scene)
        {

            var root = SearchBoneRoot(scene, scene.RootNode);

            var nodeCount = GetNodeCount(root);

            var bones = new Bone[nodeCount];

            CopyAssimpBone(root, bones, null, 0);

            return bones;

        }

        private static int CopyAssimpBone(Node node, Bone[] bones, Bone parent, int index)
        {

            bones[index] = new Bone
            {
                mName = node.Name,
                mTransformation = AssimpToXna(node.Transform),
                mParent = parent
            };

            var temporaryIndex = index;

            for (var i = 0; i < node.ChildCount; i++)
            {
                bones[index].mChildren.Add(bones[temporaryIndex + 1]);
                temporaryIndex = CopyAssimpBone(node.Children[i], bones, bones[index], temporaryIndex + 1);
            }

            return temporaryIndex;

        }

        private static int GetNodeCount(Node node)
        {

            var bonesCount = 1;

            foreach (var child in node.Children)
            {
                bonesCount += GetNodeCount(child);
            }

            return bonesCount;

        }

        private static Node SearchBoneRoot(Assimp.Scene scene, Node node)
        {

            if (IsNodeBone(scene, node))
            {
                return node.Parent;
            }
            else
            {
                foreach (var child in node.Children)
                {
                    var search = SearchBoneRoot(scene, child);
                    if (search != null)
                    {
                        return search;
                    }
                }
            }

            return null;

        }

        private static bool IsNodeBone(Assimp.Scene scene, Node node)
        {

            for (var i = 0; i < scene.MeshCount; i++)
            {
                for (var j = 0; j < scene.Meshes[i].BoneCount; j++)
                {
                    if (node.Name == scene.Meshes[i].Bones[j].Name)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        private static Matrix AssimpToXna(Matrix4x4 assimp)
        {

            var matrix = new Matrix
            {
                M11 = assimp.A1,
                M12 = assimp.B1,
                M13 = assimp.C1,
                M14 = assimp.D1,
                M21 = assimp.A2,
                M22 = assimp.B2,
                M23 = assimp.C2,
                M24 = assimp.D2,
                M31 = assimp.A3,
                M32 = assimp.B3,
                M33 = assimp.C3,
                M34 = assimp.D3,
                M41 = assimp.A4,
                M42 = assimp.B4,
                M43 = assimp.C4,
                M44 = assimp.D4
            };

            return matrix;


        }

    }

}