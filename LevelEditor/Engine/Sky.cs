using System;
using Microsoft.Xna.Framework;

namespace LevelEditor.Engine
{
    public sealed class Sky
    {

        public Light Light { get; }
        public Vector3 SunLocation { get; private set; }

        private Vector3 mSunLightColor;
        private Vector3 mMoonLightColor;

        public float mTime;

        public Sky()
        {

            Light = new Light();

            mSunLightColor = new Vector3(1.0f);
            mMoonLightColor = new Vector3(1.0f) / 3.0f;

            mTime = 12.0f;

        }

        public void Update(Camera camera)
        {

            // Calculate the light position based on the time
            mTime = mTime % 24.0f;

            // We want a sunrise at 8 am and a sunset at 8pm
            var fTime = (mTime - 8.0f) / 24.0f * 2.0f * Math.PI;
            var y = (float)Math.Sin(fTime) * 10000.0f;
            var x = (float)Math.Cos(fTime) * 10000.0f;
            var z = (float)Math.Sin(fTime) * 1000.0f;

            var location = new Vector3(x, y, z);

            SunLocation = location;

            Light.mLocation = y < 0.0f ? -location : location;

            Light.mShadow.Update(camera);

        }

    }

}
