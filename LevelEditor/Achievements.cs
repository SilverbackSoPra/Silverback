using System;
using System.IO;

namespace LevelEditor
{
    internal class Achievements
    {
        // Bools for the Achievements
        public static bool Gamemaster
        {
            get
            {
                return mGamemaster;
            }
            set
            {
                mGamemaster = value;
                Redundancy = value == true ? true : Redundancy;
            }
        }
        public static bool LumberjacksNightmare
        {
            get
            {
                return mLumberjacksNightmare;
            }
            set
            {
                mLumberjacksNightmare = value;
                Redundancy = value == true ? true : Redundancy;
            }
        }
        public static bool Speedrunner
        {
            get
            {
                return mSpeedrunner;
            }
            set
            {
                mSpeedrunner = value;
                Redundancy = value == true ? true : Redundancy;
            }
        }
        public static bool TributeToTheCreators
        {
            get
            {
                return mTributeToTheCreators;
            }
            set
            {
                mTributeToTheCreators = value;
                Redundancy = value == true ? true : Redundancy;
            }
        }

        public static bool Redundancy { get; set; }

        private static bool mGamemaster;
        private static bool mLumberjacksNightmare;
        private static bool mSpeedrunner;
        private static bool mTributeToTheCreators;

        public static void Load()
        {
            // load time from textfile

            try
            {
                using (var streamReader = new StreamReader("achievements"))
                {
                    Gamemaster = Convert.ToBoolean(streamReader.ReadLine());
                    LumberjacksNightmare = Convert.ToBoolean(streamReader.ReadLine());
                    Speedrunner = Convert.ToBoolean(streamReader.ReadLine());
                    Redundancy = Convert.ToBoolean(streamReader.ReadLine());
                    TributeToTheCreators = Convert.ToBoolean(streamReader.ReadLine());

                    streamReader.Close();
                }
            }
            catch (FileNotFoundException)
            {
                // If there doesn't exist a file we use initial values
                Gamemaster = false;
                LumberjacksNightmare = false;
                Speedrunner = false;
                Redundancy = false;
                TributeToTheCreators = false;
            }

        }

        public static void Save()
        {
            // save time in textfile
            using (var streamWriter = new StreamWriter("achievements"))
            {
                streamWriter.WriteLine(Gamemaster);
                streamWriter.WriteLine(LumberjacksNightmare);
                streamWriter.WriteLine(Speedrunner);
                streamWriter.WriteLine(Redundancy);
                streamWriter.WriteLine(TributeToTheCreators);
                streamWriter.Close();

            }

        }

    }

}
