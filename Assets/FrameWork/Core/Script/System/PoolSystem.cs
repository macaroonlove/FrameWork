using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class PoolSystem : MonoBehaviour, ISubSystem
    {
        private Dictionary<string, Stack<GameObject>> _objectPool = new Dictionary<string, Stack<GameObject>>();

        public void Initialize()
        {

        }

        public void Deinitialize()
        {
            // 오브젝트 풀 비워주기
            foreach (var stack in _objectPool.Values)
            {
                foreach (var obj in stack)
                {
                    Destroy(obj);
                }
                stack.Clear();
            }
            _objectPool.Clear();
        }

        internal GameObject Spawn(GameObject obj, Transform parent = null)
        {
            string key = obj.name;

            if (_objectPool.ContainsKey(key) && _objectPool[key].Count > 0)
            {
                GameObject poolObj = _objectPool[key].Pop();
                
                if (parent != null && poolObj.transform.parent != parent)
                    poolObj.transform.parent = parent;

                poolObj.SetActive(true);
                return poolObj;
            }
            else
            {
                if (parent == null) parent = transform;

                GameObject newObj = Instantiate(obj, parent);
                newObj.name = key;

                if (!_objectPool.ContainsKey(key))
                {
                    _objectPool[key] = new Stack<GameObject>();
                }

                return newObj;
            }
        }

        internal void DeSpawn(GameObject obj)
        {
            string key = obj.name;

            if (!_objectPool.ContainsKey(key))
            {
                _objectPool[key] = new Stack<GameObject>();
            }

            _objectPool[key].Push(obj);
            obj.SetActive(false);
        }
    }
}