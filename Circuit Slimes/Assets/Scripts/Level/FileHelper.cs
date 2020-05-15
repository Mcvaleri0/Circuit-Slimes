using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;



namespace Level
{
    public class FileHelper
    {
        public static void WriteLevel(string strToWrite, string filePath)
        {
            byte[] jsonBytes = Encoding.ASCII.GetBytes(strToWrite);

            File.WriteAllBytes(filePath, jsonBytes);
        }


        public static string LoadLevel(string filePath)
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
        }

    }
}
