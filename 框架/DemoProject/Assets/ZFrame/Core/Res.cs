using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
namespace ZFrame
{
    /// <summary>资源类</summary>
    public class Res
    {
        /// <summary>存储加载的资源obj</summary>
        private static Dictionary<string, UnityEngine.Object> objDic = new();
        /// <summary>实例化obj</summary>
        /// <param name="resNameField">ResName中的字段</param>
        /// <returns>返回实例化出的obj</returns>
        ///   例: Res.createObj(ResName.Pre_Item_Cylinder);
        public static GameObject createObj(string resNameField)
        {
            return GameObject.Instantiate(Res.objDic[resNameField] as GameObject);
        }


        
        /// <summary>无泛型加载</summary>
        public static void loadAddressableRes(string label, Action action, Action<float> percentAction = null)
        {
            MonoSubstitute.instance.SStartCoroutine(IEnum_loadAddressableRes<UnityEngine.Object>(new string[]{label}, action, percentAction));
        }
        /// <summary>无泛型加载数组</summary>
        public static void loadAddressableRes(string[] IEnumerableKeys, Action action, Action<float> percentAction = null)
        {
            MonoSubstitute.instance.SStartCoroutine(IEnum_loadAddressableRes<UnityEngine.Object>(IEnumerableKeys, action, percentAction));
        }
        /// <summary>泛型加载</summary>
        public static void loadAddressableRes<T>(string label, Action action, Action<float> percentAction = null)
        {
            MonoSubstitute.instance.SStartCoroutine(IEnum_loadAddressableRes<T>(new string[]{label}, action, percentAction));
        }
        /// <summary>泛型加载数组</summary>
        public static void loadAddressableRes<T>(string[] IEnumerableKeys, Action action, Action<float> percentAction = null)
        {
            MonoSubstitute.instance.SStartCoroutine(IEnum_loadAddressableRes<T>(IEnumerableKeys, action, percentAction));
        }
        /// <summary>协程 加载标签 </summary>
        static IEnumerator IEnum_loadAddressableRes<T>(string[] label, Action action, Action<float> percentAction)
        {
            float resCount = 0;
            //加载key的所有资源的地址
            var loadResourceLocationsHandle = Addressables.LoadResourceLocationsAsync(label as IEnumerable,Addressables.MergeMode.Union);
            if (!loadResourceLocationsHandle.IsDone)
            {
                yield return loadResourceLocationsHandle;
            }

            //这一段是为了先快速加载所需加载的资源，使得下面赋值的操作提速
            var op = Addressables.LoadAssetsAsync<UnityEngine.Object>(label as IEnumerable, obj =>
            {
                resCount++;
                if (percentAction != null)
                {
                    if (resCount / loadResourceLocationsHandle.Result.Count >= 1)
                    {
                        percentAction(0.9f);
                    }
                    else
                    {
                        percentAction(resCount / loadResourceLocationsHandle.Result.Count);
                    }
                }
            },Addressables.MergeMode.Union);
            if (!op.IsDone)
            {
                yield return op;
            }

            resCount = 0;
            //加载handle列表
            List<AsyncOperationHandle> opList = new List<AsyncOperationHandle>();
            foreach (IResourceLocation location in loadResourceLocationsHandle.Result)
            {
                AsyncOperationHandle<T> loadAssetHandle
                = Addressables.LoadAssetAsync<T>(location);
                loadAssetHandle.Completed +=
                    obj =>
                    {
                        // Debug.Log("location.PrimaryKey"+location.PrimaryKey+obj.Result+"type:"+obj.Result.GetType());
                        Res.objDic[location.PrimaryKey] = obj.Result as UnityEngine.Object;
                    };
                opList.Add(loadAssetHandle);
            }

            //加载handle列表总进度
            var groupOp = Addressables.ResourceManager.CreateGenericGroupOperation(opList);
            if (!groupOp.IsDone)
            {
                yield return groupOp;
            }
            yield return null;


            Addressables.Release(loadResourceLocationsHandle);
            if (percentAction != null)
            {
                percentAction(1);
            }

            var resStr="";
            foreach(var str in label){
                resStr+=str+"|";
            }

            // Debug.Log("Res【" + resStr.Substring(0,resStr.Length-1) + "】loaded completed");
            action();
        }
        /// <summary>获取资源 </summary>
        /// <param name="resName">ResName 字符串</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public static T getRes<T>(string resName)
        {
            if (!Res.objDic.ContainsKey(resName))
            {
                return default(T);
            }

#if UNITY_ANDROID
            if (typeof(T) == typeof(Sprite) && Res.objDic[resName] is Texture2D)
            {
                Texture2D texture = Res.objDic[resName] as Texture2D;
                return (T)(object)Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
#endif
            return (T)(object)Res.objDic[resName];

        }
    }
}