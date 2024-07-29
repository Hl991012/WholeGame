namespace GameFrame
{
    public class Singleton<T> where T : new()
    {
        private static T _instance;

        protected Singleton() { }
    
        private static object lockObj = new ();

        public static T Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                    return _instance;
                }
            }
        }
    }
}
