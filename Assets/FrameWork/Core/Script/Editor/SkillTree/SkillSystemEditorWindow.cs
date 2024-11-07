using FrameWork;
using System;
using System.Collections.Generic;
using System.IO;
using Temporary.Core;
using UnityEditor;
using UnityEngine;

namespace Temporary.Editor
{
    public class SkillSystemEditorWindow : EditorWindow
    {
        private int selectedTab = 0;
        private int selectedSkillIndex = 0;
        private Vector2 scrollPosition;
        private List<Tuple<SkillTemplate, Texture2D>> skillTemplates = new List<Tuple<SkillTemplate, Texture2D>>();

        private Texture2D emptyTexture2D;

        [MenuItem("Window/스킬 시스템")]
        public static void Open()
        {
            var window = GetWindow<SkillSystemEditorWindow>();
            window.titleContent = new GUIContent("스킬 시스템");
        }

        private void OnGUI()
        {
            DrawTab();
        }

        private void DrawTab()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selectedTab == 0, "스킬트리", "Button")) selectedTab = 0;
            if (GUILayout.Toggle(selectedTab == 1, "스킬", "Button")) selectedTab = 1;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            switch (selectedTab)
            {
                case 0:
                    DrawSkillTreeTab();
                    break;
                case 1:
                    DrawSkillTab();
                    break;
            }
        }

        private void DrawSkillTreeTab()
        {
            GUILayout.Label("스킬트리 탭", EditorStyles.boldLabel);
        }

        private void DrawSkillTab()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("스킬 추가"))
            {
                AddSkillTemplate();
            }
            if (GUILayout.Button("스킬 삭제"))
            {
                DeleteSelectedSkillTemplate();
            }
            if (GUILayout.Button("스킬 탐색"))
            {
                LoadSkillTemplates();
            }

            DrawLine();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            var skillCatalog = new GUIStyle(GUI.skin.button);
            skillCatalog.alignment = TextAnchor.MiddleLeft;
            skillCatalog.padding = new RectOffset(5, 5, 5, 5);
            skillCatalog.margin = new RectOffset(5, 5, -2, -2);
            skillCatalog.border = new RectOffset(0, 0, 0, 0);
            skillCatalog.fixedWidth = GUI.skin.box.fixedWidth;
            skillCatalog.fixedHeight = GUI.skin.box.fixedHeight;

            for (int i = 0; i < skillTemplates.Count; i++)
            {
                bool isSelected = (selectedSkillIndex == i);

                var text = "  " + skillTemplates[i].Item1.displayName;
                text = text.Substring(0, Mathf.Min(text.Length, 13));
                GUIContent content = new GUIContent(text, skillTemplates[i].Item2);
                
                if (GUILayout.Toggle(isSelected, content, skillCatalog))
                {
                    selectedSkillIndex = i;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (skillTemplates.Count > 0 && selectedSkillIndex < skillTemplates.Count)
            {
                SkillTemplate selectedSkill = skillTemplates[selectedSkillIndex].Item1;

                var editor = UnityEditor.Editor.CreateEditor(selectedSkill);
                editor.OnInspectorGUI();
                editor.serializedObject.ApplyModifiedProperties();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void AddSkillTemplate()
        {
            // 스킬 템플릿 생성
            SkillTemplate newSkill = CreateInstance<SkillTemplate>();

            // 에셋 저장
            string defaultPath = "Assets/FrameWork/Core/GameData/Skill";
            string path = EditorUtility.SaveFilePanelInProject("FrameWork/Core/GameData/Skill", "Skill_", "asset", "스킬 템플릿은 FrameWork/Core/GameData/Skill 위치에 저장됩니다..", defaultPath);
            if (!string.IsNullOrEmpty(path))
            {
                newSkill.displayName = Path.GetFileNameWithoutExtension(path);

                AssetDatabase.CreateAsset(newSkill, path);
                AssetDatabase.SaveAssets();
                LoadSkillTemplates();
            }
        }

        private void DeleteSelectedSkillTemplate()
        {
            if (skillTemplates.Count > 0)
            {
                SkillTemplate selectedSkill = skillTemplates[selectedSkillIndex].Item1;
                string assetPath = AssetDatabase.GetAssetPath(selectedSkill);
                skillTemplates.RemoveAt(selectedSkillIndex);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        private void LoadSkillTemplates()
        {
            emptyTexture2D = CreateTexture(Color.gray);
            skillTemplates.Clear();
            string[] guids = AssetDatabase.FindAssets("t:SkillTemplate");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SkillTemplate skill = AssetDatabase.LoadAssetAtPath<SkillTemplate>(path);

                var texture = (skill.sprite == null) ? emptyTexture2D : skill.sprite.texture;
                texture = texture.ResizeTexture(30, 30);

                skillTemplates.Add(new Tuple<SkillTemplate, Texture2D>(skill, texture));
            }
        }

        #region 유틸리티
        private void DrawLine()
        {
            GUILayout.Space(5);
            Rect rect = GUILayoutUtility.GetRect(0, 1);
            EditorGUI.DrawRect(rect, Color.gray);
            GUILayout.Space(5);
        }

        Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(64, 64);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
        #endregion
    }
}
