using System;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace LevelEditor.Engine
{
    [Serializable()]
    public sealed class Terrain : IActor, ISerializable
    {

        public Actor Actor { get; }
        public int Size { get; }

        [XmlIgnore]
        public Grass mGrass;

        private const float TerrainScale = 1.0f;
        private readonly float TerrainMaxHeight = 40.0f;
        private const float TextureRepitions = 1.0f;

        private readonly int mHeight;
        private readonly int mWidth;

        public readonly string mHeightMapPath;
        public readonly string mTexturePath;

        private readonly ContentManager mContent;
        private readonly GraphicsDevice mGraphics;

        
        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public Terrain(ContentManager content, GraphicsDevice device, string heightMapPath, string texturePath)
        {

            mContent = content;
            mGraphics = device;

            mHeightMapPath = heightMapPath;
            mTexturePath = texturePath;

            var meshData = new MeshData();

            var heightMapStream = new FileStream(heightMapPath, FileMode.Open, FileAccess.Read);
            var heightmap = Texture2D.FromStream(device, heightMapStream);
            heightMapStream.Close();

            mWidth = heightmap.Width;
            mHeight = heightmap.Height;

            meshData.mVertices = new VertexPositionTexture[mWidth * mHeight];
            meshData.mIndices = new int[(mWidth * 2 + 2) * (mHeight - 1)];

            var subData = new MeshData.SubData();
            var material = new Material();

            var textureStream = new FileStream(texturePath, FileMode.Open, FileAccess.Read);
            material.mDiffuseMap = Texture2D.FromStream(device, textureStream);
            material.DiffuseMapStream = textureStream;
            material.mGraphicsDevice = device;
            textureStream.Close();

            material.mDiffuseColor = new Vector3(1.0f);

            subData.mIndicesOffset = 0;
            subData.mMaterialIndex = 0;
            subData.mNumPrimitives = (heightmap.Width * 2 + 2) * (heightmap.Height - 1);

            meshData.mMaterials.Add(material);
            meshData.mSubDatas.Add(subData);

            meshData.mPrimitiveType = PrimitiveType.TriangleStrip;
            meshData.mTotalNumPrimitives = (heightmap.Width * 2 + 2) * (heightmap.Height - 1);

            meshData.mBoundingSphere.Radius = (float)Math.Sqrt(Math.Pow(heightmap.Width / 2.0f, 2) + Math.Pow(heightmap.Height / 2.0f, 2));
            meshData.mIsTerrain = true;

            Actor = new Actor(new Mesh.Mesh(mGraphics, meshData),null) {mCastShadow = false};           
            Size = mWidth;

            LoadVertices(heightmap);

            LoadIndices();

            Actor.mMesh.UpdateData();

            mGrass = new Grass(this);
            mGrass.Generate();

        }       

        public void Reload()
        {

            var meshData = Actor.mMesh.mMeshData;

            var heightMapStream = new FileStream(mHeightMapPath, FileMode.Open, FileAccess.Read);
            var heightmap = Texture2D.FromStream(mGraphics, heightMapStream);
            heightMapStream.Close();

            if (heightmap.Height == mHeight && heightmap.Width == mWidth)
            {

                var textureStream = new FileStream(mTexturePath, FileMode.Open, FileAccess.Read);
                meshData.mMaterials[0].mDiffuseMap = Texture2D.FromStream(mGraphics, textureStream);
                textureStream.Close();

                LoadVertices(heightmap);

                Actor.mMesh.UpdateData();

                mGrass.Generate();

            }

        }

        public float GetHeight(Vector3 location)
        {
            
			float height;
            
            var x = mWidth / 2f + location.X;
            var z = mHeight / 2f + location.Z;

            if (x < 0 || z < 0 || x >= (mWidth - 1) * TerrainScale || z >= (mHeight - 1) * TerrainScale)
            {
                return 0.0f;
            }

            var position = new Vector2((int)Math.Floor(x / TerrainScale), (int)Math.Floor(z / TerrainScale));

            if (position.X < 0 || position.Y < 0 ||
                (int)position.X >= mWidth * TerrainScale || (int)position.Y >= mHeight * TerrainScale)
            {
                return 0.0f;
            }

            var coord = new Vector2(x % TerrainScale / TerrainScale, z % TerrainScale / TerrainScale);

            if (coord.X > coord.Y)
            {
                height = BarryCentric(new Vector3(0.0f,
                        Actor.mMesh.mMeshData.mVertices[(int)(position.X + mWidth * position.Y)].Position.Y,
                        0.0f),
                    new Vector3(1.0f, Actor.mMesh.mMeshData.mVertices[(int)(position.X + 1 + mWidth * position.Y)].Position.Y, 0.0f),
                    new Vector3(1.0f, Actor.mMesh.mMeshData.mVertices[(int)(position.X + 1 + mWidth * (position.Y + 1))].Position.Y, 1.0f),
                    coord);
            }
            else
            {
                height = BarryCentric(new Vector3(0.0f, Actor.mMesh.mMeshData.mVertices[(int)(position.X + mWidth * position.Y)].Position.Y, 0.0f),
                    new Vector3(1.0f, Actor.mMesh.mMeshData.mVertices[(int)(position.X + 1 + mWidth * (position.Y + 1))].Position.Y, 1.0f),
                    new Vector3(0.0f, Actor.mMesh.mMeshData.mVertices[(int)(position.X + mWidth * (position.Y + 1))].Position.Y, 1.0f),
                    coord);
            }

            return height;

        }

        public float GetSlope(Vector3 location)
        {

            var x = mWidth / 2f + location.X;
            var z = mHeight / 2f + location.Z;

            if (x < 0 || z < 0 || x >= (mWidth - 1) * TerrainScale || z >= (mHeight - 1) * TerrainScale)
            {
                return 0.0f;
            }

            var position = new Vector2((int)Math.Floor(x / TerrainScale), (int)Math.Floor(z / TerrainScale));

            if (position.X < 0 || position.Y < 0 ||
                (int)position.X >= mWidth * TerrainScale || (int)position.Y >= mHeight * TerrainScale)
            {
                return 0.0f;
            }

            var northWest = new Vector3(0.0f, Actor.mMesh.mMeshData.mVertices[(int)(position.X + mWidth * position.Y)].Position.Y, 0.0f);
            var northEast = new Vector3(1.0f, Actor.mMesh.mMeshData.mVertices[(int)(position.X + 1 + mWidth * position.Y)].Position.Y, 0.0f);
            var southWest = new Vector3(0.0f, Actor.mMesh.mMeshData.mVertices[(int)(position.X + mWidth * (position.Y + 1))].Position.Y, 1.0f);
            var southEast = new Vector3(1.0f, Actor.mMesh.mMeshData.mVertices[(int)(position.X + 1 + mWidth * (position.Y + 1))].Position.Y, 1.0f);

            var min = northWest;
            min = min.Y > northEast.Y ? northEast : min;
            min = min.Y > southWest.Y ? southWest : min;
            min = min.Y > southEast.Y ? southEast : min;

            var max = northWest;
            max = max.Y < northEast.Y ? northEast : max;
            max = max.Y < southWest.Y ? southWest : max;
            max = max.Y < southEast.Y ? southEast : max;

            var distance = Vector2.Distance(new Vector2(min.X, min.Z), new Vector2(max.X, max.Z));
            var difference = max.Y - min.Y;

            return (float)Math.Sinh(difference / distance);

        }

        private float GetHeightFromData(VertexPositionTexture[] vertices, int x, int z)
        {

            if (x < mWidth && z < mHeight && z >= 0 && x >= 0)
            {
                return vertices[z * mWidth + x].Position.Y;
            }
            else
            {
                return vertices[0].Position.Y;
            }

        }

        private static float BarryCentric(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 pos)
        {
            var det = (p2.Z - p3.Z) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Z - p3.Z);
            var l1 = ((p2.Z - p3.Z) * (pos.X - p3.X) + (p3.X - p2.X) * (pos.Y - p3.Z)) / det;
            var l2 = ((p3.Z - p1.Z) * (pos.X - p3.X) + (p1.X - p3.X) * (pos.Y - p3.Z)) / det;
            var l3 = 1.0f - l1 - l2;
            return l1 * p1.Y + l2 * p2.Y + l3 * p3.Y;
        }

        private void LoadVertices(Texture2D heightMap)
        {

            var newHeightVal = new Color[mWidth * mHeight];
            heightMap.GetData(newHeightVal);
            var meshData = Actor.mMesh.mMeshData;

            for (int z = 0; z < mHeight; z++)
            {
                for (int x = 0; x < mWidth; x++)
                {
                    //Position
                    meshData.mVertices[x + z * mWidth].Position.X = (x - mWidth / 2.0f) * TerrainScale;
                    meshData.mVertices[x + z * mWidth].Position.Y = newHeightVal[x + z * mWidth].G / 255.0f * TerrainMaxHeight;
                    meshData.mVertices[x + z * mWidth].Position.Z = (z - mHeight / 2.0f) * TerrainScale;

                    //Texture
                    meshData.mVertices[x + z * mWidth].TextureCoordinate.X = (float)x / mWidth * TextureRepitions;
                    meshData.mVertices[x + z * mWidth].TextureCoordinate.Y = (float)z / mHeight * TextureRepitions;
                }
            }

        }

        private void LoadIndices()
        {
            var meshData = Actor.mMesh.mMeshData;

            //Calculates the triangle strip
            var i = 0;

            for (int z = 0; z < mHeight - 1; z++)
            {

                meshData.mIndices[i++] = z * mWidth;

                for (int x = 0; x < mWidth; x++)
                {
                    meshData.mIndices[i++] = z * mWidth + x;
                    meshData.mIndices[i++] = (z + 1) * mWidth + x;

                }

                meshData.mIndices[i++] = (z + 1) * mWidth + (mWidth - 1);


            }

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Actor", Actor);
            info.AddValue("Size", Size);
            info.AddValue("mGrass", mGrass);
            info.AddValue("TerrainMaxHeight", TerrainMaxHeight);
            info.AddValue("mHeight", mHeight);
            info.AddValue("mWidth", mWidth);
            info.AddValue("mHeightMapPath", mHeightMapPath);
            info.AddValue("mTexturePath", mTexturePath);
            info.AddValue("mContent", mContent);
            info.AddValue("mGraphics", mGraphics);
        }

        public Terrain(SerializationInfo info, StreamingContext context)
        {
            Actor = (Actor)info.GetValue("Actor", typeof(Actor));
            Size = (int)info.GetValue("Size", typeof(int));
            mGrass = (Grass)info.GetValue("mGrass", typeof(Grass));
            TerrainMaxHeight = (float)info.GetValue("TerrainMaxHeight", typeof(float));
            mHeight = (int)info.GetValue("mHeight", typeof(int));
            mWidth = (int)info.GetValue("mWidth", typeof(int));
            mHeightMapPath = (string)info.GetValue("mHeightMapPath", typeof(string));
            mTexturePath = (string)info.GetValue("mTexturePath", typeof(string));
            mContent = (ContentManager)info.GetValue("mContent", typeof(ContentManager));
            mGraphics = (GraphicsDevice)info.GetValue("mGraphics", typeof(GraphicsDevice));
        }

        public Terrain()
        {
            mGrass = new Grass(this);
        }
        [OnDeserialized()]
        internal void OnSerializedMethod(StreamingContext context)
        {
            // Setting this as parent property for Child object
            Actor.IActor = this;
        }
    }

}
