using System.Reflection;
using UnityEngine;
using System;
using System.Collections.Generic;
namespace ZFrame
{
    /// <summary>unity框架入口</summary>
    public class Entrance
    {
        public Entrance()
        {
            Core.RootNode= new GameObject("ZFrame");
            GameObject.DontDestroyOnLoad(Core.RootNode);
            
            Core.RootNode.AddComponent<MonoSubstitute>();
            ZFrame.EventSystem.GetRegisteEventFunc(Assembly.GetExecutingAssembly());
            ZFrame.UI.init();
        }
        
        
    }
}