using UnityEngine;

namespace Cos.Common
{
    public class TSingletonMono<T>: CachedMono where T : TSingletonMono<T>
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if(instance != null)
                    return instance;

                instance = FindObjectOfType(typeof(T)) as T;

                if(instance == null)
                {
                    string instanceGameObjectName = typeof(T).ToString();

                    instance = new GameObject(instanceGameObjectName).AddComponent<T>();
                    DontDestroyOnLoad(instance);
                    instance.OnCreateInstance();
                }

                return instance;
            }
        }

        public static bool HasInstance()
        {
            return instance != null;
        }

        public static void TryResetInstance()
        {
            if(instance != null)
            {
                DestroyImmediate(instance.gameObject);
                instance = default(T);
            }
        }

        protected virtual void OnCreateInstance()
        {
        }

        protected virtual void OnResetInstance()
        {

        }

        protected override void OnDestroy()
        {
            instance.OnResetInstance();
            instance = default(T);
            base.OnDestroy();            
        }
    }

}
