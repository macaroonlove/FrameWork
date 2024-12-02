using UnityEngine;
using XNode;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Skill/Skill Tree Graph")]
    public class SkillTreeGraph : NodeGraph
    {
        public string displayName;

        public int nodeLevelCount;
        public int nodeIndexCount;

        private void OnValidate()
        {
            nodeLevelCount = 0;
            nodeIndexCount = 0;
            
            foreach (var node in nodes)
            {
                if (node is ActiveSkillNode activeSkillNode)
                {
                    nodeLevelCount = Mathf.Max(nodeLevelCount, activeSkillNode.level);
                    nodeIndexCount = Mathf.Max(nodeIndexCount, activeSkillNode.index);
                }
                else if (node is PassiveSkillNode passiveSkillNode)
                {
                    nodeLevelCount = Mathf.Max(nodeLevelCount, passiveSkillNode.level);
                    nodeIndexCount = Mathf.Max(nodeIndexCount, passiveSkillNode.index);
                }
            }

            nodeLevelCount++;
            nodeIndexCount++;
        }
    }
}

#if UNITY_EDITOR
namespace Temporary.Editor
{
    using Temporary.Core;
    using UnityEditor;
    using UnityEngine;
    using XNodeEditor;

    [CustomEditor(typeof(SkillTreeGraph))]
    public class SkillTreeGraphEditor : NodeGraphEditor
    {
        private SkillTreeGraph graph;

        public override void OnGUI()
        {
            graph = (SkillTreeGraph)target;

            base.OnGUI();
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("적용하기"), false, Update);
            base.AddContextMenuItems(menu);
        }

        private void Update()
        {
            graph.nodeLevelCount = 0;
            graph.nodeIndexCount = 0;

            foreach (var node in graph.nodes)
            {
                if (node is ActiveSkillNode activeSkillNode)
                {
                    graph.nodeLevelCount = Mathf.Max(graph.nodeLevelCount, activeSkillNode.level);
                    graph.nodeIndexCount = Mathf.Max(graph.nodeIndexCount, activeSkillNode.index);
                }
                else if (node is PassiveSkillNode passiveSkillNode)
                {
                    graph.nodeLevelCount = Mathf.Max(graph.nodeLevelCount, passiveSkillNode.level);
                    graph.nodeIndexCount = Mathf.Max(graph.nodeIndexCount, passiveSkillNode.index);
                }
            }

            graph.nodeLevelCount++;
            graph.nodeIndexCount++;
        }
    }
}
#endif