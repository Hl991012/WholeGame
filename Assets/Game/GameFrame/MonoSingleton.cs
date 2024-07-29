using System;
using UnityEngine;

namespace GameFrame
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;

        protected MonoSingleton () { }

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static T Instance
        {
            get
            {
                _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;
                var obj = new GameObject
                {
                    name = typeof(T).ToString()
                };
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }
    }
}
