using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LevelEditor.Engine.Animation
{
    internal sealed class Bone
    {

        public string mName;

        public Bone mParent;
        public readonly List<Bone> mChildren;

        public Matrix mOffset;
        public Matrix mTransformation;

        public Matrix mMatrix;
        public Vector3 mPosition;

        public Bone()
        {

            mChildren = new List<Bone>();

        }

        public static void CalculateBoneBaseTransformation(Bone[] bones, ref Matrix[] transformations)
        {

            for (var i = 0; i < bones.Length; i++)
            {

                var bone = bones[i];

                if (bone.mParent != null)
                {
                    bone.mMatrix = bone.mTransformation * bone.mParent.mMatrix;
                }
                else
                {
                    bone.mMatrix = bone.mTransformation;
                }

                transformations[i] = bone.mOffset * bone.mMatrix;

            }

        }

    }
}
