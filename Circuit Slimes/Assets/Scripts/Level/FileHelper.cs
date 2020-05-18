using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;



namespace Level
{
    public class FileHelper
    {
        #region /* Paths */

        public const string LEVELS_PATH = "Levels";
        public const string ITEMS_PATH  = "Prefabs/Board Items";
        public const string BUTTON_PATH = "Prefabs/Button";

        private const string SAVE_PATH = "Assets/Resources/Levels";

        #endregion


        #region /* Files Names */

        public const string EMPTY_LEVEL = "EmptyLevel";

        #endregion



        #region === File Methods ===

        public static void WriteLevel(string strToWrite, string name)
        {
            string path = PrepareSavePath(name);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(strToWrite);

            File.WriteAllBytes(path, jsonBytes);

            #if UNITY_EDITOR
                AssetDatabase.Refresh();
            #endif
        }


        public static string LoadLevel(string name)
        {
            string filePath = Path.Combine(LEVELS_PATH, name);

            TextAsset file = Resources.Load<TextAsset>(filePath);
            string str = file.ToString();

            return str;
        }


        /*public static string LoadLevel(string filePath)
        {
            string jsonString;

            if (Application.platform == RuntimePlatform.Android)
            {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                www.SendWebRequest();

                while (!www.isDone) { }

                jsonString = www.downloadHandler.text;
            }
            else
            {
                byte[] jsonBytes = File.ReadAllBytes(filePath);
                jsonString = Encoding.ASCII.GetString(jsonBytes);
            }

            return jsonString;
        }*/


        public static List<string> GetFileList(string pathInResources)
        {
            Object[] files = Resources.LoadAll(pathInResources);
            List<string> res = new List<string>();

            foreach (Object file in files)
            {
                res.Add(file.name);
            }

            return res;
        }

        #endregion


        #region === Path Methods ===

        private static string PrepareSavePath(string fileName)
            {
            #if UNITY_EDITOR
                return Path.Combine(SAVE_PATH, fileName + ".json");
            #else
                return Path.Combine(Application.persistentDataPath, LEVELS_PATH, fileName + ".json");
            #endif
        }

        #endregion
    }
}
