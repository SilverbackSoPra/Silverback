using System;

namespace LevelEditor
{
    public static class LumberChoice
    {
        private const int Hidden = 3;
        private static double[] sApes = new double[4] {0.25d,0.25d,0.25d,0.25d};
        private static double[,] sWeights = new double[Hidden, sApes.Length+1];
        private static double sKillerRatio = 0.5d;
        private static int[] mNumApes = new int[4] {0,0,0,0};
        private static bool isInitialized = false;

        /// <summary>
        /// Use this to tell the AI that you added a single SubApe.
        /// </summary>
        /// <param name="index">1 for chimp, 2 for orang, 3 for capucin, 4 for gibbon</param>
        public static void AddApe(int index)
        {
            mNumApes[index-1]++;
            SetApes(mNumApes[0],mNumApes[1],mNumApes[2],mNumApes[3]);
        }

        /// <summary>
        /// Use this to tell the AI how many of each type of SubApe the player has, so it can react accordingly.
        /// </summary>
        /// <param name="chimps">Number of Chimpanzees</param>
        /// <param name="orangs">Number of Orang-Utans</param>
        /// <param name="capucins">Number of Capucins</param>
        /// <param name="gibbons">Number of Gibbons</param>
        public static void SetApes(int chimps, int orangs, int capucins, int gibbons)
        {
            var total = chimps + orangs + capucins + gibbons;
            sApes = new double[]{(double)chimps/total, (double) orangs/total, (double) capucins/total, (double)gibbons/total};
            SetRatio();
        }

        private static void SetRatio()
        {
            if (!isInitialized && Hidden==3 && sApes.Length==4)
            {
                sWeights = new double[,]
                {
                    {-2.0d, 1.0d, -1.0d, 2.0d, 4.0d},
                    {2.0d, 1.0d, -2.0d, -1.0d, -2.0d},
                    {1.0d, -2.0d, 2.0d, 1.0d, -1.0d}
                };
            } 
            var outActivation = 0.0d;
            double[] hiddenNeurons = new double[Hidden];
            for (var i = 0; i < Hidden; i++)
            {
                var activation = 0.0d;
                for (var j = 0; j < sApes.Length; j++)
                {
                    activation += sApes[j] * sWeights[i, j];
                }
                hiddenNeurons[i] = 1.0d / (1.0d + Math.Exp(-activation));
                outActivation += hiddenNeurons[i] * sWeights[i, sApes.Length];
            }

            sKillerRatio = 1.0d / (1.0d + Math.Exp(-outActivation));
        }

        /// <summary>
        /// This tells you how many Lumberjacks a hut with the given total HP should spawn, the rest becoming DoubleAxeKillers.
        /// </summary>
        /// <param name="hp">HP of the hut asking this, i.e. total spare spawn tickets.</param>
        /// <returns>The number of Lumberjacks to be spawned by the hut, as opposed to DoubleAxeKillers.</returns>
        public static int NumLumberjacks(int hp)
        {
            return (int) Math.Round(hp * (1.0d - sKillerRatio));
        }
    }
}