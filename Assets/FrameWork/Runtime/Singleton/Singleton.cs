using UnityEngine;

namespace FrameWork
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(nameof(T)).AddComponent<T>();
                    instance.Initialize();
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                instance.Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void Initialize()
        {

        }
    }
}