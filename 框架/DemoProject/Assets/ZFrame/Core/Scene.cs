using System.Globalization;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
/// <summary>
/// 场景管理器
/// </summary>
namespace ZFrame
{
    public enum LoadType
    {
        Resources,
        Addressable,
    }

    public class Scene
    {
        /// <summary>
        /// 跳转场景
        /// </summary>
        /// <param name="sceneName">场景resName</param>
        public static void goScene(string sceneName, Action action , LoadType loadType = LoadType.Addressable)
        {
            if (loadType == LoadType.Addressable)
            {
                MonoSubstitute.instance.SStartCoroutine(loadAddressableScene(sceneName, action));
            }
            else if(loadType==LoadType.Resources)
            {
                MonoSubstitute.instance.SStartCoroutine(loadResourcesScene(sceneName, action));
            }
        }

        static IEnumerator loadAddressableScene(string sceneName, Action action)
        {
           
            var op = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (op.Status != AsyncOperationStatus.Succeeded)
            {
                // Debug.Log("op.percentComplete:"+op.PercentComplete);
                yield return op;
            }
            // Addressables.Release(op);
            if (action != null)
            {
                action();
            }
            UI.addToBaseCamera();
        }

        static IEnumerator loadResourcesScene(string scene, Action action)
        {
            var op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            if (!op.isDone)
            {
                // Debug.Log("op.progress:"+op.progress);
                yield return op;
            }
            if (action != null)
            {
                action();
            }
            UI.addToBaseCamera();
        }
    }
}