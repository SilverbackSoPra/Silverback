namespace LevelEditor.Engine.Postprocessing
{
    internal sealed class Bloom
    {

        public bool Activated { get; set; }

        public float mThreshold;
        public float mPower;
        public float mIntensity;

        public int mPasses;

        public Bloom()
        {

            mThreshold = 0.5f;
            mPower = 1.0f;
            mIntensity = 1.0f;

            mPasses = 5;

            Activated = true;

        }

    }

}
