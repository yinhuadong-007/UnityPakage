using System.Globalization;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
namespace ZFrame
{
    public interface IUIData<T>
    {
        T viewData
        {
            get;
            set;
        }
    }

    /// <summary>UI管理器</summary>
    public partial class UI
    {
        /// <summary>UICanvas节点 </summary>
        private static GameObject nodeUICanvas;
        private static Dictionary<string, UnityEngine.Object> _dic_ViewHashCode = new();
        public static GameObject container
        {
            get { return nodeUICanvas; }
        }


        public static Camera uiCamera;
        public static Camera overUI3DCamera;

        /// <summary>UI基础资源初始化</summary>
        public static void init()
        {
              nodeUICanvas = GameObject.Instantiate(Resources.Load<GameObject>("ZFrame/UICanvas"), Core.RootNode.transform);
            nodeUICanvas.name = "UICanvas(ZFrame)";
            GameObject.DontDestroyOnLoad(nodeUICanvas);
            addToBaseCamera();
            adaptScreen();
            layerInit();
        }
        /// <summary>适配屏幕 </summary>
        private static void adaptScreen()
        {
            var scaler = nodeUICanvas.GetComponent<CanvasScaler>();
            if (Screen.width / scaler.referenceResolution.x < Screen.height / scaler.referenceResolution.y)
            {
                scaler.matchWidthOrHeight = 0f;
            }
            else
            {
                scaler.matchWidthOrHeight = 1f;
            }
        }

        /// <summary>把uiCamera加入到主摄像机中 </summary>
        public static void addToBaseCamera()
        {
            overUI3DCamera=nodeUICanvas.transform.Find("ObjOnUICamera").GetComponent<Camera>();
            overUI3DCamera.transform.SetParent(nodeUICanvas.transform.parent);
             

            foreach (Camera camera in Camera.allCameras)
            {
                if (camera.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Base)
                {
                    camera.GetUniversalAdditionalCameraData().cameraStack.Add(nodeUICanvas.GetComponent<Canvas>().worldCamera);
                    uiCamera = nodeUICanvas.GetComponent<Canvas>().worldCamera;
                    //主摄像机剔除ui层渲染
                    camera.cullingMask &= ~(1 << 5);

                    camera.GetUniversalAdditionalCameraData().cameraStack.Add(overUI3DCamera);
                    break;
                }
            }

           

        }

        /// <summary>添加ui界面</summary>
        /// <param name="viewName">ui界面名称</param>
        public static void addView(string viewName,Layer layer=Layer.bottom)
        {
            if (_dic_ViewHashCode.ContainsKey(viewName) && _dic_ViewHashCode[viewName] != null)
            {
                return;
            }
            var view = GameObject.Instantiate(Res.getRes<GameObject>(viewName),nodeUICanvas.transform); 
            _dic_ViewHashCode[viewName] = view;
            addToLayer(layer,view);
        }

        /// <summary>添加ui界面(带数据)</summary>
        /// <param name="viewName">ui界面名称</param>
        public static void addView(string viewName, dynamic viewData,Layer layer=Layer.bottom)
        {
            if (_dic_ViewHashCode.ContainsKey(viewName) && _dic_ViewHashCode[viewName] != null)
            {
                return;
            }
            var view = GameObject.Instantiate(Res.getRes<GameObject>(viewName),nodeUICanvas.transform);
            _dic_ViewHashCode[viewName] = view;
            addToLayer(layer,view);
            //注入viewData
            var c = view.GetComponent<MonoBehaviour>();
            Type type = view.GetComponent<MonoBehaviour>().GetType();
            var method = type.GetMethod("set_viewData");
            method.Invoke(c, new object[] { viewData });
        }

        /// <summary>返回ui类</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T getView<T>()
        {
            return nodeUICanvas.GetComponentInChildren<T>();
        }

        public static Component getView(Type t)
        {
            return nodeUICanvas.GetComponentInChildren(t);
        }

        public static void destroyView<T>() where T:MonoBehaviour{
            if(ZFrame.UI.getView<T>()!=null){
                GameObject.Destroy(ZFrame.UI.getView<T>().gameObject);
            }
        }

          public static void destroyView(Type t) {
            if(ZFrame.UI.getView(t)!=null){
                GameObject.Destroy(ZFrame.UI.getView(t).gameObject);
            }
        }



        /// <summary>加载界面</summary>
        /// <typeparam name="T">界面View</typeparam>
        public static void addView<T>(dynamic receiveData = null) 
        {
            var newMediator = Activator.CreateInstance(typeof(T)) ;
            newMediator.GetType().GetMethod("init").Invoke(newMediator,new object[]{receiveData});

            // newMediator.init(new object[] { viewData }); 
        }
    }
}