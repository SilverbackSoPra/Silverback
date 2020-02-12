using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Engine.Helper;
using Microsoft.Xna.Framework;

namespace LevelEditor.Engine
{
    public class Grass
    {

        public const int GrassPatchSize = 4;
        public const int GrassPatchInstancesCount = 25;

        public class Patch
        {

            public Vector3 mPosition;
            public int mPatchInstancesOffset;
            public float mRadius = (float)Math.Sqrt(2.0 * Math.Pow(GrassPatchSize / 2.0, 2.0));
            public float mDistance;
            public float mSlope;
            public bool mRender;

            public Patch(Vector3 position, int patchInstancesOffset)
            {
                mPosition = position;
                mPatchInstancesOffset = patchInstancesOffset;
                mDistance = 0.0f;
                mRender = true;
            }

            public void Generate(Terrain terrain, GrassInstancing[] instances, Random random)
            {

                mSlope = terrain.GetSlope(mPosition);

                for (var i = 0; i < GrassPatchInstancesCount; i++)
                {
                    var x = 2.0f * (float)random.NextDouble() - 1.0f;
                    var z = 2.0f * (float)random.NextDouble() - 1.0f;
                    var position = new Vector3(x, 0.0f, z) * GrassPatchSize / 2.0f + mPosition;

                    position.Y = terrain.GetHeight(position);
                    instances[mPatchInstancesOffset + i].Position = position;
                }
            }

        }


        public GrassInstancing[] mGrassInstances;
        public Patch[] mPatches;
        public bool mActivated;

        private Terrain mTerrain;
        private Random mRandom = new Random();

        public Grass(Terrain terrain)
        {

            mActivated = true;

            var patchesPerLine = terrain.Size / GrassPatchSize;

            mGrassInstances = new GrassInstancing[patchesPerLine * patchesPerLine * GrassPatchInstancesCount];
            mTerrain = terrain;
            mPatches = new Patch[patchesPerLine * patchesPerLine];

            for (var i = 0; i < patchesPerLine; i++)
            {
                for (var j = 0; j < patchesPerLine; j++)
                {
                    var position = new Vector3(i * GrassPatchSize + GrassPatchSize / 2.0f - 128.0f, 0.0f, j * GrassPatchSize + GrassPatchSize / 2.0f - 128.0f);
                    var offset = (i * patchesPerLine + j) * GrassPatchInstancesCount;
                    position.Y = mTerrain.GetHeight(position);
                    mPatches[i * patchesPerLine + j] = new Patch(position, offset);
                }
            }

        }

        public void Generate()
        {

            foreach (var patch in mPatches)
            {
                patch.Generate(mTerrain, mGrassInstances, mRandom);
            }

        }
    }
}