namespace U14.Event
{
    /// <summary>
    /// 事件工具
    /// <para>ZhangYu 2019-03-04</para>
    /// </summary>
    public static class EventUtil
    {

        /// <summary> 事件派发器 </summary>
        private static EventDispatcher dispatcher = new EventDispatcher();

        /// <summary> 添加事件监听器 </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventHandler">事件处理器</param>
        /// <param name="target">事件目标</param>
        public static void AddListener(string eventType, EventListener.EventHandler eventHandler, object target)
        {
            dispatcher.AddListener(eventType, eventHandler, target);
        }

        /// <summary> 移除指定事件监听器 </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventHandler">事件处理器</param>
        /// <param name="target">事件目标</param>
        public static void RemoveListener(string eventType, EventListener.EventHandler eventHandler, object target)
        {
            dispatcher.RemoveListener(eventType, eventHandler, target);
        }

        /// <summary> 移除目标身上的所有事件监听器 </summary>
        /// <param name="target">目标对象</param>
        public static void RemoveListener(object target)
        {
            dispatcher.RemoveListener(target);
        }

        /// <summary> 是否已经拥有该类型的事件 </summary>
        /// <param name="eventType">事件类型</param>
        public static bool HasListener(string eventType, object target = null)
        {
            return dispatcher.HasListener(eventType, target);
        }

        /// <summary> 派发事件 </summary>
        /// <param name="eventType">事件类型</param>
        public static void DispatchEvent(string eventType, params object[] args)
        {
            dispatcher.DispatchEvent(eventType, args);
        }

        /// <summary> 清理所有事件监听器 </summary>
        public static void Clear()
        {
            dispatcher.Clear();
        }
    }

    //使用
    ////// <summary> 游戏事件类型 </summary>
    // public static class GameEventType
    // {
    //     /// <summary> 通知敌人开启碰撞 </summary>
    //     public const string Enemy_Change_Collider_State = "Enemy_Change_Collider_State";
    // }

}