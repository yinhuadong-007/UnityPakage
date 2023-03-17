namespace U14.Event
{
    /// <summary> 事件参数
    /// <para>ZhangYu 2019-03-05</para>
    /// </summary>
    public class EventArgs
    {
        /// <summary> 事件类型 </summary>
        public readonly string type;
        /// <summary> 事件参数 </summary>
        public readonly object[] args;

        public EventArgs(string type)
        {
            this.type = type;
        }

        public EventArgs(string type, params object[] args)
        {
            this.type = type;
            this.args = args;
        }

    }
}