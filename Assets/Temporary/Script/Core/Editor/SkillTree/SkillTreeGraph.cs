using UnityEngine;
using XNode;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Skill/Skill Tree Graph", fileName = "SkillTree_", order = 2)]
    public class SkillTreeGraph : NodeGraph
    {
        public string displayName;

        public Vector2Int gridSize;
        public Vector2Int nodeSpacing;
    }
}

#if UNITY_EDITOR
namespace Temporary.Editor
{
    using Temporary.Core;
    using XNodeEditor;

    [CustomNodeGraphEditor(typeof(SkillTreeGraph))]
    public class SkillTreeGraphEditor : NodeGraphEditor
    {
        public override NoodlePath GetNoodlePath(NodePort output, NodePort input)
        {
            return NoodlePath.Straight;
        }
    }
}
#endif