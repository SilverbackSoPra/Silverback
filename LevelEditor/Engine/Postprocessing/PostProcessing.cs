
namespace LevelEditor.Engine.Postprocessing
{
    /// <summary>
    /// Represents the post-processing applied to a scene.
    /// </summary>
    public sealed class PostProcessing
    {

        public readonly Fxaa mFxaa;
        public readonly Bloom mBloom;

        public readonly float mSaturation;

        /// <summary>
        /// Constructs a <see cref="PostProcessing"/>.
        /// </summary>
        public PostProcessing()
        {

            mSaturation = 1.0f;

            mFxaa = new Fxaa();
            mBloom = new Bloom();

        }

    }

}
