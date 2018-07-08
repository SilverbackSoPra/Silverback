using System;
using System.IO;

namespace LevelEditor
{
    internal class Achievements
    {
        // Bools for the Achievements
        private static bool Gamemaster { get; set; }
        private static bool Lumberjacksnightmare { get; set; }
        private static bool Speedrunner { get; set; }
        private static bool Redundancy { get; set; }
        private static bool Tributetothecreators { get; set; }
        public static void Load()
        {
            // load time from textfile

            try
            {
                using (var streamReader = new StreamReader("achievements"))
                {
                    Gamemaster = Convert.ToBoolean(streamReader.ReadLine());
                    Lumberjacksnightmare = Convert.ToBoolean(streamReader.ReadLine());
                    Speedrunner = Convert.ToBoolean(streamReader.ReadLine());
                    Redundancy = Convert.ToBoolean(streamReader.ReadLine());
                    Tributetothecreators = Convert.ToBoolean(streamReader.ReadLine());

                    streamReader.Close();
                }
            }
            catch (FileNotFoundException)
            {
                // If there doesn't exist a file we use initial values
                Gamemaster = false;
                Lumberjacksnightmare = false;
                Speedrunner = false;
                Redundancy = false;
                Tributetothecreators = false;
            }

        }

        public static void Save()
        {
            // save time in textfile
            using (var streamWriter = new StreamWriter("achievements"))
            {
                streamWriter.WriteLine(Gamemaster);
                streamWriter.WriteLine(Lumberjacksnightmare);
                streamWriter.WriteLine(Speedrunner);
                streamWriter.WriteLine(Redundancy);
                streamWriter.WriteLine(Tributetothecreators);
                streamWriter.Close();

            }

        }

    }

}
