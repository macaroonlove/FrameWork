using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Temporary.Core
{
    [System.Serializable]
    public class SkillNode : Node
    {
        [HideInInspector] public SkillTemplate skillTemplate;
        [Output] public int level;
        [HideInInspector] public int index;
        [Input(dynamicPortList = true)] public List<int> prevSkill;

        protected override void Init()
        {
            base.Init();
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

    [CustomNodeEditor(typeof(SkillNode))]
    public class SkillNodeEditor : NodeEditor
    {
        public override void OnHeaderGUI()
        {
            SkillNode skillNode = (SkillNode)target;

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Space(5);

            if (skillNode.skillTemplate != null)
            {
                GUILayout.Label(skillNode.skillTemplate.displayName, style);
            }
            else
            {
                GUILayout.Label("Skill Template�� �߰����ּ���.", style);
            }

            GUILayout.Space(10);
        }

        public override void OnBodyGUI()
        {
            SkillNode skillNode = (SkillNode)target;
            SkillTemplate skillTemplate = skillNode.skillTemplate;

            GUILayout.BeginHorizontal();
            GUILayout.Label("��ų", GUILayout.Width(60));
            skillNode.skillTemplate = EditorGUILayout.ObjectField(skillNode.skillTemplate, typeof(SkillTemplate), false) as SkillTemplate;
            GUILayout.EndHorizontal();

            if (skillTemplate != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (skillTemplate.sprite != null)
                {
                    GUILayout.Label(skillTemplate.sprite.texture, GUILayout.Width(64), GUILayout.Height(64));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("����", GUILayout.Width(60));
                skillNode.level = EditorGUILayout.IntField(skillNode.level);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("����", GUILayout.Width(60));
                skillNode.index = EditorGUILayout.IntField(skillNode.index);
                GUILayout.EndHorizontal();
            }

            base.OnBodyGUI();
        }
    }
}
#endif