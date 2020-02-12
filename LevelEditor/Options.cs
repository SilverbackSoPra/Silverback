using System;
using System.IO;

namespace LevelEditor
{
    internal static class Options
    {

        public static bool Fxaa { get; set; }
        public static int MusicVolume { get; set; }
        public static int SoundEffectVolume { get; set; }
        public static float Brightness { get; set; }
        public static int Resolution
        {
            get { return sResolution; }
            set {
                sResolution = value;
                ResolutionWidth = sScreenWidth * value / 100;
                ResolutionHeight = sScreenHeight * value / 100;
            }
        }
        public static int ResolutionWidth { get; private set; }
        public static int ResolutionHeight { get; private set; }

        public static int GraphicsQuality
        {
            get { return sGraphicsQuality; }
            set
            {
                sGraphicsQuality = value;
                switch (value)
                {
                    case 0:
                        Shadows = false;
                        ShadowSamples = 0;
                        ShadowResolution = 4;
                        ShadowDistance = 0.0f;
                        ShadowBias = 0.0f;
                        BloomPasses = 0;
                        Bloom = false;
                        ViewingDistance = 50.0f;
                        break;
                    case 1:
                        Shadows = true;
                        ShadowSamples = 1;
                        ShadowDistance = 30.0f;
                        ShadowResolution = 1024;
                        ShadowBias = 0.002f;
                        BloomPasses = 0;
                        ViewingDistance = 75.0f;
                        Bloom = false;
                        break;
                    case 2:
                        Shadows = true;
                        ShadowSamples = 8;
                        ShadowDistance = 60.0f;
                        ShadowResolution = 2048;
                        ShadowBias = 0.002f;
                        BloomPasses = 3;
                        ViewingDistance = 125.0f;
                        Bloom = true;
                        break;
                    case 3:
                        Shadows = true;
                        ShadowSamples = 16;
                        ShadowDistance = 90.0f;
                        ShadowResolution = 4096;
                        ShadowBias = 0.001f;
                        BloomPasses = 5;
                        ViewingDistance = 200.0f;
                        Bloom = true;
                        break;
                    case 4:
                        Shadows = true;
                        ShadowSamples = 16;
                        ShadowDistance = 120.0f;
                        ShadowResolution = 8192;
                        ShadowBias = 0.0005f;
                        BloomPasses = 5;
                        Bloom = true;
                        ViewingDistance = 250.0f;
                        break;
                }
            }
        }

        // Shadow settings
        public static bool Shadows { get; private set; }
        public static int ShadowSamples { get; private set; }
        public static int ShadowResolution { get; private set; }
        public static float ShadowDistance { get; private set; }
        public static float ShadowBias { get; private set; }

        // Bloom settings
        public static bool Bloom { get; private set; }
        public static int BloomPasses { get; private set; }

        // Camera settins
        public static float ViewingDistance { get; private set; }

        private static int sResolution;
        private static int sScreenWidth = 1920;
        private static int sScreenHeight = 1080;

        private static int sGraphicsQuality = 2;

        public static void Load()
        {
            // load time from textfile          

            try
            {
                using (var streamReader = new StreamReader("options"))
                {
                    Fxaa = Convert.ToBoolean(streamReader.ReadLine());
                    MusicVolume = Convert.ToInt32(streamReader.ReadLine());
                    SoundEffectVolume = Convert.ToInt32(streamReader.ReadLine());
                    Resolution = Convert.ToInt32(streamReader.ReadLine());
                    GraphicsQuality = Convert.ToInt32(streamReader.ReadLine());
                    Brightness = Convert.ToSingle( streamReader.ReadLine()); // test
                    streamReader.Close();
                }
            }
            catch (FileNotFoundException)
            {
                // If there doesn't exist a file we use some standard settings
                Fxaa = true;
                MusicVolume = 100;
                SoundEffectVolume = 100;
                Resolution = 100;
                GraphicsQuality = 2;
                Brightness = 1.0f;
            }

        }

        public static void Save()
        {
            // save time in textfile (udin
            using (var streamWriter = new StreamWriter("options"))
            {
                streamWriter.WriteLine(Fxaa);
                streamWriter.WriteLine(MusicVolume);
                streamWriter.WriteLine(SoundEffectVolume);
                streamWriter.WriteLine(Resolution);
                streamWriter.WriteLine(GraphicsQuality);
                streamWriter.WriteLine(Brightness);
                streamWriter.Close();

            }

        }

        public static void ScreenResolution(int width, int height)
        {
            sScreenWidth = width;
            sScreenHeight = height;
        }

    }

}
