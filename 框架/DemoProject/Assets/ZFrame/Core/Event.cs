using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

namespace ZFrame
{







    /**废弃不用 纯看看语法
    继承事件类-不灵活
     public class EventReceiveClass
    {
        public EventReceiveClass()
        {
            Type type = this.GetType();
            if (!classDic.ContainsKey(type))
            {
                classDic.Add(type, new List<object>());
            }
            classDic[type].Add(this);
        }
        public void removeEvent()
        {
            var list = classDic[this.GetType()];
            list.Remove(this);
        }
        ~EventReceiveClass()
        {
            UnityEngine.Debug.Log("~Clear EventReceiveClass");
            var list = classDic[this.GetType()];
            list.Remove(this);
        }
    }

    //event 事件
    public delegate void GlobalEventHandler();

    public static event GlobalEventHandler globalOrder;
    /// <summary>发送事件</summary>
    /// <param name="eventName">事件字符串</param>

    public static void sendGlobalEvent(string eventName)
    {
        if(globalOrder!=null){
            globalOrder.Invoke();
        }
    }
    public static void addEvent(GlobalEventHandler action)
    {
        globalOrder += action;
    }
    public static void removeEvent(GlobalEventHandler action)
    {
        globalOrder -= action;
    }
    */

    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class EventReceiveAttribute : System.Attribute
    {
        public string methodDesc;
        public EventReceiveAttribute(string getMethodDesc)
        {
            this.methodDesc = getMethodDesc;
        }
    }

    public interface IEventReceiveFunc
    {
        void checkEventSwitch(bool bl);
    }

    public class EventSystem
    {
        private static Dictionary<Type, Dictionary<string, string>> _eventDic = new();
        public static Dictionary<Type, List<object>> classDic = new();
        public static void AddEvent(object t){
             getReceiveClass(t, true);
        }
        public static void RemoveEvent(object t){
             getReceiveClass(t, false);
        }
        public static void getReceiveClass(object t, bool bl)
        {
            Type type = t.GetType();

            if (bl == true)
            {
                if (!classDic.ContainsKey(type))
                {
                    classDic.Add(type, new List<object>());
                }
                classDic[type].Add(t);
            }
            else
            {
                if (classDic.ContainsKey(t.GetType()))
                {
                    var list = classDic[t.GetType()];
                    if (list != null)
                    {
                        list.Remove(t);
                    }
                }
            }
        }

        public static void GetRegisteEventFunc(Assembly ass = null)
        {
            _eventDic.Clear();
            classDic.Clear();
            if (ass == null)
            {
                ass = Assembly.GetCallingAssembly();
            }
            Type[] assTypes = ass.GetTypes();
            foreach (var assType in assTypes)
            {
                foreach (var method in assType.GetMethods())
                {
                    foreach (var attr in Attribute.GetCustomAttributes(method))
                    {
                        if (attr is EventReceiveAttribute)
                        {
                            string methodDesc = (attr as EventReceiveAttribute).methodDesc;
                            if (!_eventDic.ContainsKey(assType))
                            {
                                _eventDic.Add(assType, new Dictionary<string, string>());
                            }
                            _eventDic[assType][methodDesc] = method.Name;
                        }
                    }
                }
            }
        }

        public static void goEvent(string getMethodDesc, params object[] data)
        {

            List<Action> ActionList = new List<Action>();

            foreach (var eventFuncData in _eventDic)
            {
                foreach (var storage_methodDesc in eventFuncData.Value)
                {
                    if (storage_methodDesc.Key == getMethodDesc)
                    {
                        Type registerType = eventFuncData.Key;
                        if (classDic.ContainsKey(registerType))
                        {
                            foreach (var classObj in classDic[registerType])
                            {
                                if (classObj == null)
                                {
                                    classDic[registerType].Remove(classObj);
                                    continue;
                                }
                                data = data == null ? null : data;
                                string methodName = eventFuncData.Value[getMethodDesc];
                                // classObj.GetType().GetMethod(methodName).Invoke(classObj, data);

                                ActionList.Add(() =>
                                {
                                    classObj.GetType().GetMethod(methodName).Invoke(classObj, data);
                                });
                            }
                        }
                        // return;
                    }
                }
            }
            foreach (Action a in ActionList)
            {
                a();
            }
            ActionList.Clear();
        }
    }
}