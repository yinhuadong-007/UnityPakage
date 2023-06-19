namespace U14.Common
{
    public class SingleTemplate<T> where T : class, new()
    {
        // private static readonly object syslock = new object();

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    // lock (syslock)
                    // {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                    // }
                }
                return instance;
            }
        }

        protected static void __free()
        {
            instance = null;
        }
    }
}


