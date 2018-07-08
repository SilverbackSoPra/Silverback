using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LevelEditor
{
    public class DataSaver
    {
        //Save Data
        public static void SaveData<T>(T dataToSave, string dataFileName)
        {


            var tempPath = Path.Combine(Application.LocalUserAppDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + ".txt");

            //Convert To Json then to bytes
            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSave);
            var jsonByte = Encoding.ASCII.GetBytes(jsonData);

            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                var path = Path.GetDirectoryName(tempPath);
                if (path != null)
                {
                    Directory.CreateDirectory(path);
                }
            }

            try
            {
                File.WriteAllBytes(tempPath, jsonByte);
                Console.WriteLine("Saved Data to: " + tempPath.Replace("/", "\\"));
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to write " + dataFileName);
                // Debug.LogWarning("Failed To PlayerInfo Data to: " + tempPath.Replace("/", "\\"));
                // Debug.LogWarning("Error: " + e.Message);
            }
        }

        //Load Data
        public static T LoadData<T>(string dataFileName)
        {
            var tempPath = Path.Combine(Application.LocalUserAppDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + ".txt");

            //Exit if Directory or File does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                // Debug.LogWarning("Directory does not exist");
                return default(T);
            }

            if (!File.Exists(tempPath))
            {
                // Debug.Log("File does not exist");
                return default(T);
            }

            //Load saved Json
            byte[] jsonByte = null;
            try
            {
                jsonByte = File.ReadAllBytes(tempPath);
                // Debug.Log("Loaded Data from: " + tempPath.Replace("/", "\\"));
            }
            catch (Exception)
            {
                // Debug.LogWarning("Failed To Load Data from: " + tempPath.Replace("/", "\\"));
                // Debug.LogWarning("Error: " + e.Message);
            }

            //Convert to json string

            string jsonData = null;

            if (jsonByte != null)
            {
                jsonData = Encoding.ASCII.GetString(jsonByte);
            }

            //Convert to Object
            var resultValue = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonData);
            return (T)Convert.ChangeType(resultValue, typeof(T));
        }

        public static bool DeleteData(string dataFileName)
        {
            var success = false;

            //Load Data
            var tempPath = Path.Combine(Application.LocalUserAppDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + ".txt");

            //Exit if Directory or File does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                // Debug.LogWarning("Directory does not exist");
                return false;
            }

            if (!File.Exists(tempPath))
            {
                // Debug.Log("File does not exist");
                return false;
            }

            try
            {
                File.Delete(tempPath);
                // Debug.Log("Data deleted from: " + tempPath.Replace("/", "\\"));
                success = true;
            }
            catch (Exception)
            {
                // Debug.LogWarning("Failed To Delete Data: " + e.Message);
            }

            return success;
        }
    }
}
