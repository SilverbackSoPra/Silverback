using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.Engine.Mesh
{
    [Serializable()]
    public sealed class Material
    {
        [XmlIgnore]
        public Texture2D mDiffuseMap { get; set; }

        public string mDiffuseMapPath;

        [XmlIgnore]
        public GraphicsDevice mGraphicsDevice;

        private FileStream mDiffuseMapStream;

        public FileStream DiffuseMapStream
        {
            get { return mDiffuseMapStream; }
            set
            {
                mDiffuseMapStream = value;
                if (mGraphicsDevice == null)
                {
                    return;
                }
                mDiffuseMap = Texture2D.FromStream(mGraphicsDevice, mDiffuseMapStream);
            }
        }

        public string DiffuseMapPath
        {
            get { return mDiffuseMapPath; }
            set
            {
                mDiffuseMapPath = value;
                if (value == null)
                {
                    return;
                }

            }
        }

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
