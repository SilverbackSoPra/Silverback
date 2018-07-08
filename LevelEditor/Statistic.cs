using System;
using System.IO;

namespace LevelEditor
{
    class Statistic
    {

        public static double Time { get; set; }
        public static int EscapedApes { get; set; }
        public static int EscapedLumberjacks { get; set; }
        public static int Win { get; set; }
        public static int Lost { get; set; }
        public static int MinimalTime { get; set; }

        public static void Load()
        {
            try
            {
                using (var streamReader = new StreamReader("statistics"))
                {
                    // load content from textfile
                    Time = Convert.ToDouble(streamReader.ReadLine());
                    EscapedApes = Convert.ToInt32(streamReader.ReadLine());
                    EscapedLumberjacks = Convert.ToInt32(streamReader.ReadLine());
                    Win = Convert.ToInt32(streamReader.ReadLine());
                    Lost = Convert.ToInt32(streamReader.ReadLine());
                    MinimalTime = Convert.ToInt32(streamReader.ReadLine());

                    streamReader.Close();
                }
            }

            catch (FileNotFoundException)
            {
                // If there doesn't exist a file we start with zero
                Time = 0;
                EscapedApes = 0;
                EscapedLumberjacks = 0;
                Win = 0;
                Lost = 0;
                MinimalTime = 0;
            }

        }

        public static void Save()
        {
            using (var streamWriter = new StreamWriter("statistics"))
            {
                // save content in textfile
                streamWriter.WriteLine(Time);
                streamWriter.WriteLine(EscapedApes);
                streamWriter.WriteLine(EscapedLumberjacks);
                streamWriter.WriteLine(Win);
                streamWriter.WriteLine(Lost);
                streamWriter.WriteLine(MinimalTime);

                streamWriter.Close();

            }
               
        }

    }
}
