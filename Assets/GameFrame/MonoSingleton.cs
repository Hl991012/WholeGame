using UnityEngine;

namespace GameFrame
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        protected MonoSingleton () { }

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var obj = new GameObject();
                _instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
                return _instance;
            }
        }
    }
}
