

namespace U14.Event
{
    using System.Collections.Generic;

    /// <summary>
    /// 事件派发器
    /// <para>ZhangYu 2019-03-05</para>
    /// </summary>
    public class EventDispatcher
    {

        /// <summary> 事件Map </summary>
        // private Dictionary<string, EventListener> dic = new Dictionary<string, EventListener>();
        private Dictionary<object, Dictionary<string, EventListener>> dic2 = new Dictionary<object, Dictionary<string, EventListener>>();

        /// <summary> 添加事件监听器 </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventHandler">事件处理器</param>
        public void AddListener(string eventType, EventListener.EventHandler eventHandler, object target)
        {
            // EventListener invoker;
            // if (!dic.TryGetValue(eventType, out invoker))
            // {
            //     invoker = new EventListener();
            //     dic.Add(eventType, invoker);
            // }
            // invoker.eventHandler += eventHandler;

            Dictionary<string, EventListener> dic = new Dictionary<string, EventListener>();
            if (!dic2.TryGetValue(target, out dic))
            {
                dic = new Dictionary<string, EventListener>();
                dic2.Add(target, dic);
            }
            EventListener invoker;
            if (!dic.TryGetValue(eventType, out invoker))
            {
                invoker = new EventListener();
                dic.Add(eventType, invoker);
            }
            invoker.eventHandler += eventHandler;
        }

        /// <summary> 移除事件监听器 </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventHandler">事件处理器</param>
        /// <param name="target">事件目标</param>
        public void RemoveListener(string eventType, EventListener.EventHandler eventHandler, object target)
        {
            // EventListener invoker;
            // if (dic.TryGetValue(eventType, out invoker)) invoker.eventHandler -= eventHandler;

            Dictionary<string, EventListener> dic = new Dictionary<string, EventListener>();
            if (dic2.TryGetValue(target, out dic))
            {
                EventListener invoker;
                if (dic.TryGetValue(eventType, out invoker)) invoker.eventHandler -= eventHandler;
            }
        }

        /// <summary> 移除事件监听器 </summary>
        /// <param name="target">事件目标</param>
        public void RemoveListener(object target)
        {
            // EventListener invoker;
            // if (dic.TryGetValue(eventType, out invoker)) invoker.eventHandler -= eventHandler;

            Dictionary<string, EventListener> dic = new Dictionary<string, EventListener>();
            if (dic2.TryGetValue(target, out dic))
            {
                foreach (EventListener value in dic.Values)
                {
                    value.Clear();
                }
                dic.Clear();
            }
        }

        /// <summary> 是否已经拥有该类型的事件 </summary>
        /// <param name="eventType">事件类型</param>
        public bool HasListener(string eventType, object target)
        {
            // return dic.ContainsKey(eventType);

            Dictionary<string, EventListener> dic = new Dictionary<string, EventListener>();
            if (target != null)
            {
                if (dic2.TryGetValue(target, out dic))
                {
                    return dic.ContainsKey(eventType);
                }
            }
            else
            {
                foreach (var item in dic2)
                {
                    dic = item.Value;
                    if (dic.ContainsKey(eventType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary> 派发事件 </summary>
        /// <param name="eventType">事件类型</param>
        public void DispatchEvent(string eventType, params object[] args)
        {
            // EventListener invoker;
            // if (dic.TryGetValue(eventType, out invoker))
            // {
            //     EventArgs evt;
            //     if (args == null || args.Length == 0)
            //     {
            //         evt = new EventArgs(eventType);
            //     }
            //     else
            //     {
            //         evt = new EventArgs(eventType, args);
            //     }
            //     invoker.Invoke(evt);
            // }


            foreach (var item in dic2)
            {
                Dictionary<string, EventListener> dic = item.Value;
                EventListener invoker;
                if (dic.TryGetValue(eventType, out invoker))
                {
                    EventArgs evt;
                    if (args == null || args.Length == 0)
                    {
                        evt = new EventArgs(eventType);
                    }
                    else
                    {
                        evt = new EventArgs(eventType, args);
                    }
                    invoker.Invoke(evt);
                }
            }

        }

        /// <summary> 清理所有事件监听器 </summary>
        public void Clear()
        {
            // foreach (EventListener value in dic.Values)
            // {
            //     value.Clear();
            // }
            // dic.Clear();
            foreach (var item in dic2)
            {
                Dictionary<string, EventListener> dic = item.Value;
                foreach (EventListener value in dic.Values)
                {
                    value.Clear();
                }
                dic.Clear();
            }
            dic2.Clear();
        }
    }

}