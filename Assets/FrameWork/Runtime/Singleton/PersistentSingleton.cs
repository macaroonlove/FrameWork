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
                Debug.LogError("�ı����� �ʴ� ������Ʈ�� �θ� ����� �մϴ�.");
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
                    Debug.Log($"�̹� �̱����� �����Ͽ� �����˴ϴ�. {this.gameObject.name}");
                    Destroy(this.gameObject);
                }
            }
        }

        protected virtual void Initialize()
        {

        }
    }
}