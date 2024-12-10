using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// 액티브 아이템 효과를 적용시키는 시스템
    /// </summary>
    public class ActiveItemSystem : MonoBehaviour, ISubSystem
    {
        [SerializeField] private List<UIActiveItemExecuteButton> _executeButton = new List<UIActiveItemExecuteButton>();
        [SerializeField, ReadOnly] private List<ActiveItemTemplate> _selectedItems = new List<ActiveItemTemplate>();

        [SerializeField, ReadOnly] private List<ActiveItemTemplate> _items = new List<ActiveItemTemplate>();

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private List<ActiveItemTemplate> _debugItems = new List<ActiveItemTemplate>();
#endif

        public void Initialize()
        {
#if UNITY_EDITOR
            _selectedItems.AddRange(_debugItems);
#endif

            int maxCount = Mathf.Min(_executeButton.Count, _selectedItems.Count);
            for (int i = 0; i < maxCount; i++)
            {
                if (_selectedItems[i] == null)
                {
                    _executeButton[i].Hide(true);
                }
                else
                {
                    _executeButton[i].Show(_selectedItems[i]);
                }
            }

            for (int i = maxCount; i < _executeButton.Count; i++)
            {
                _executeButton[i].Hide(true);
            }
        }

        public void Deinitialize()
        {
            
        }

        /// <summary>
        /// 아이템 추가
        /// </summary>
        public void AddItem(ActiveItemTemplate template)
        {
            if (_items.Contains(template))
            {
#if UNITY_EDITOR
                Debug.LogError($"아이템이 중복되었습니다. {template.displayName}");
#endif
                return;
            }

            _items.Add(template);
        }

        private void OnDestroy()
        {            
            _items.Clear();
        }
    }
}