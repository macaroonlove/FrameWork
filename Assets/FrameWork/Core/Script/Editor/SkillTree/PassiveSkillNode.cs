using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Temporary.Core
{
    [System.Serializable]
    public class PassiveSkillNode : SkillNode
    {
        [HideInInspector] public PassiveSkillTemplate skillTemplate;
    }
}

#if UNITY_EDITOR
namespace Temporary.Editor
{
    using Temporary.Core;
    using UnityEditor;
    using UnityEngine;
    using XNodeEditor;

    [CustomNodeEditor(typeof(PassiveSkillNode))]
    public class PassiveSkillNodeEditor : NodeEditor
    {
        public override void OnHeaderGUI()
        {
            PassiveSkillNode skillNode = (PassiveSkillNode)target;

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
                GUILayout.Label("Skill Template를 추가해주세요.", style);
            }

            GUILayout.Space(10);
        }

        public override void OnBodyGUI()
        {
            PassiveSkillNode skillNode = (PassiveSkillNode)target;
            PassiveSkillTemplate skillTemplate = skillNode.skillTemplate;

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬", GUILayout.Width(60));
            skillNode.skillTemplate = EditorGUILayout.ObjectField(skillNode.skillTemplate, typeof(PassiveSkillTemplate), false) as PassiveSkillTemplate;
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
                GUILayout.Label("레벨", GUILayout.Width(60));
                skillNode.level = EditorGUILayout.IntField(skillNode.level);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("순서", GUILayout.Width(60));
                skillNode.index = EditorGUILayout.IntField(skillNode.index);
                GUILayout.EndHorizontal();
            }

            base.OnBodyGUI();
        }
    }
}
#endif