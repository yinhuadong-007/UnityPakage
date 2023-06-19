using System.Net.Http.Headers;
using System.IO.Enumeration;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
namespace ZFrame
{
    public struct StorageData
    {
        public string fileName;
        public object data;
    }

    public class Storage
    {
        /// <summary>存储本地数据字典</summary>
        public static Dictionary<Type, StorageData> _dic = new();
        /// <summary>
        /// 读取本地配置数据，并初始化字典
        /// </summary>
        /// <param name="fileName">文件名，例:"localData.json"</param>
        /// <typeparam name="T">json数组格式</typeparam>
        public static void GenerateJsonData<T>(string fileName)
        {
            checkConfig(fileName);
            readJsonData<T>(fileName);
        }
        /// <summary>读取本地数据存入字典</summary>
        public static void readJsonData<T>(string fileName)
        {
            var path = Application.persistentDataPath + "/" + fileName;
            StreamReader streamReader = new StreamReader(path);
            string str = streamReader.ReadToEnd();
            Storage._dic[typeof(T)] = new StorageData() { fileName = fileName, data = JsonUtility.FromJson<T>(str) };
            streamReader.Close();
        }

        //  IL2cpp模式下可用 通过web请求的方式获取文件的二进制数据
        public static void checkConfig(string fileName)
        {
            // Debug.Log("storageData:" + Application.persistentDataPath);
            string dirPath = Application.persistentDataPath;
            string filePath = dirPath + "/" + fileName;
            byte[] jsonByte = ReadStreamAssetBytes(fileName);
#if UNITY_EDITOR
            //unity编辑器下，每次都要把当前streamAssets的配置文件覆盖到persistentData位置
            if (!Directory.Exists(dirPath))
            {
                //创建文件夹
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(filePath, jsonByte);
#else

        if (!File.Exists(filePath))
        {
            Debug.Log("not exist config");
            if (!Directory.Exists(dirPath))
            {
                //创建文件夹
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(filePath, jsonByte);
        }
#endif
        }

        public static byte[] ReadStreamAssetBytes(string file)
        {
            var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, file);
#if !UNITY_EDITOR && UNITY_ANDROID
            // return new AndroidJavaClass("com.u14.Helper").CallStatic<byte[]>("readAsset", file);
            var request = UnityEngine.Networking.UnityWebRequest.Get(new System.Uri(filePath));
            request.SendWebRequest();
            if(request.error!=null){
                Debug.LogError("ReadStreamAssetsFail => "+file);
            }else{
                while (!request.downloadHandler.isDone){}
            }
            return request.downloadHandler.data;
#else
            return File.ReadAllBytes(filePath);
#endif
        }

        public static T getData<T>() where T : class
        {
            return Storage._dic[typeof(T)].data as T;
        }
        public static void saveData<T>()
        {
            string str = JsonUtility.ToJson(Storage._dic[typeof(T)].data);
            var path = Application.persistentDataPath + "/" + Storage._dic[typeof(T)].fileName;
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.Write(str);
            streamWriter.Flush();
            streamWriter.Close();

#if UNITY_EDITOR
            try
            {
                var path2 = Application.streamingAssetsPath + "/localData.json";
                StreamWriter streamWriter2 = new StreamWriter(path2);
                streamWriter2.Write(str);
                streamWriter2.Flush();
                streamWriter2.Close();
            }
            catch
            {
                Debug.Log("write config error");
            }
#endif
        }
    }
}