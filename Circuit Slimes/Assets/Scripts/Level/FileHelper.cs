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
        #region /* Constants */

        private const string SAVE_PATH   = "Assets/Resources/Levels";
        private const string LEVELS_PATH = "Levels";

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
