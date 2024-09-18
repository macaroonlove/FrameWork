using UnityEngine;

namespace FrameWork
{
    public class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        instance = new GameObject(nameof(T)).AddComponent<T>();
                        instance.Initialize();
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (this.transform.parent != null)
            {
                this.transform.SetParent(null);
                Debug.LogError("파괴되지 않는 오브젝트는 부모가 없어야 합니다.");
            }

            if (instance == null || instance == this)
            {
                instance = this as T;
                DontDestroyOnLoad(transform.gameObject);
            }
            else
            {
                if (this != instance)
                {
                    Debug.Log($"이미 싱글톤이 존재하여 삭제됩니다. {this.gameObject.name}");
                    Destroy(this.gameObject);
                }
            }
        }

        protected virtual void Initialize()
        {

        }
    }
}