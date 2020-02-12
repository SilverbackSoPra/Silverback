
namespace LevelEditor.Engine.Postprocessing
{

    /// <summary>
    /// Represents the fast approximate anti-aliasing algorithm.
    /// </summary>
    public sealed class Fxaa
    {

        public bool Activated { get; set; }

        public float mLumaThreshold;
        public float mLumaThresholdMin;

        public bool mDebugMode;
        

        /// <summary>
        /// Constructs a <see cref="Fxaa"/>
        /// </summary>
        public Fxaa()
        {

            mLumaThreshold = 1.0f / 8.0f;
            mLumaThresholdMin = 1 / 32.0f;

            mDebugMode = false;
            Activated = true;

        }

    }
}
