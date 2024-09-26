using FrameWork.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/Agent", fileName = "AgentLibrary", order = 0)]
    public class AgentLibraryTemplate : ScriptableObject
    {
        public List<AgentTemplate> templates = new List<AgentTemplate>();

        [Header("필터")]
        [Label("모든 유닛"), SerializeField] private bool _isAll;
        [Condition("_isAll", false)]
        [Label("소유 여부"), SerializeField] private bool _isOwned;

        [ContextMenu("필터로 AgentTemplate 찾기")]
        public void FindAllRefresh()
        {
            templates.Clear();
            var AssetsGUIDArray = AssetDatabase.FindAssets("t:AgentTemplate", new[] { "Assets/" });
            foreach (var guid in AssetsGUIDArray)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath(path, typeof(AgentTemplate)) as AgentTemplate;

                bool isInclude = _isAll;
                isInclude |= _isOwned && template.isOwned;

                if (template != null && isInclude)
                {
                    templates.Add(template);
                }
            }
            // 변경 사항을 저장
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}
