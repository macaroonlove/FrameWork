using FrameWork;
using System;
using System.Collections.Generic;
using System.IO;
using Temporary.Core;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Temporary.Editor
{
    public class SkillSystemEditorWindow : EditorWindow
    {
        private int selectedTab = 0;
        private Texture2D emptyTexture2D;

        #region ��ų
        private int selectedSkillIndex = 0;
        private Vector2 skillScrollPosition;
        private List<Tuple<SkillTemplate, Texture2D>> skillTemplates = new List<Tuple<SkillTemplate, Texture2D>>();
        #endregion
        #region ��ų Ʈ��
        private int selectedSkillTreeIndex = 0;
        private Vector2 skillTreeScrollPosition;
        private List<SkillTreeGraph> skillTreeTemplates = new List<SkillTreeGraph>();
        #endregion

        [MenuItem("Window/��ų �ý���")]
        public static void Open()
        {
            var window = GetWindow<SkillSystemEditorWindow>();
            window.titleContent = new GUIContent("��ų �ý���");
        }

        private void OnGUI()
        {
            DrawTab();
        }

        private void CreateGUI()
        {
        }

        private void DrawTab()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selectedTab == 0, "��ųƮ��", "Button")) selectedTab = 0;
            if (GUILayout.Toggle(selectedTab == 1, "��ų", "Button")) selectedTab = 1;
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

        #region ��ų
        private void DrawSkillTab()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("��ų �߰�"))
            {
                AddSkillTemplate();
            }
            if (GUILayout.Button("��ų ����"))
            {
                DeleteSelectedSkillTemplate();
            }
            if (GUILayout.Button("��ų Ž��"))
            {
                LoadSkillTemplates();
            }

            DrawLine();

            skillScrollPosition = GUILayout.BeginScrollView(skillScrollPosition, false, true);

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
            // ��ų ���ø� ����
            SkillTemplate newSkill = CreateInstance<SkillTemplate>();

            // ���� ����
            string defaultPath = "Assets/FrameWork/Core/GameData/Skill";
            string path = EditorUtility.SaveFilePanelInProject("FrameWork/Core/GameData/Skill", "Skill_", "asset", "��ų ���ø��� FrameWork/Core/GameData/Skill ��ġ�� ����˴ϴ�..", defaultPath);
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
        #endregion

        #region ��ųƮ��
        private void DrawSkillTreeTab()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("��ų Ʈ�� �߰�"))
            {
                AddSkillTreeTemplate();
            }
            if (GUILayout.Button("��ų Ʈ�� ����"))
            {
                DeleteSelectedSkillTreeTemplate();
            }
            if (GUILayout.Button("��ų Ʈ�� Ž��"))
            {
                LoadSkillTreeTemplates();
            }

            DrawLine();

            skillTreeScrollPosition = GUILayout.BeginScrollView(skillTreeScrollPosition, false, true);

            var skillCatalog = new GUIStyle(GUI.skin.button);
            skillCatalog.alignment = TextAnchor.MiddleLeft;
            skillCatalog.padding = new RectOffset(5, 5, 5, 5);
            skillCatalog.margin = new RectOffset(5, 5, -2, -2);
            skillCatalog.border = new RectOffset(0, 0, 0, 0);
            skillCatalog.fixedWidth = GUI.skin.box.fixedWidth;
            skillCatalog.fixedHeight = GUI.skin.box.fixedHeight;

            for (int i = 0; i < skillTreeTemplates.Count; i++)
            {
                bool isSelected = (selectedSkillTreeIndex == i);

                var text = "  " + skillTreeTemplates[i].displayName;
                text = text.Substring(0, Mathf.Min(text.Length, 18));

                if (GUILayout.Toggle(isSelected, text, skillCatalog))
                {
                    selectedSkillTreeIndex = i;   
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (GUILayout.Button("������ ����"))
            {
                NodeEditorWindow.Open(skillTreeTemplates[selectedSkillTreeIndex]);
            }
            
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void AddSkillTreeTemplate()
        {
            // ��ų ���ø� ����
            SkillTreeGraph newSkill = CreateInstance<SkillTreeGraph>();

            // ���� ����
            string defaultPath = "Assets/FrameWork/Core/GameData/SkillTree";
            string path = EditorUtility.SaveFilePanelInProject("FrameWork/Core/GameData/SkillTree", "SkillTree_", "asset", "��ų Ʈ�� ���ø��� �����մϴ�.", defaultPath);
            if (!string.IsNullOrEmpty(path))
            {
                newSkill.displayName = Path.GetFileNameWithoutExtension(path);

                AssetDatabase.CreateAsset(newSkill, path);
                AssetDatabase.SaveAssets();
                LoadSkillTemplates();
            }
        }

        private void DeleteSelectedSkillTreeTemplate()
        {
            if (skillTreeTemplates.Count > 0)
            {
                SkillTreeGraph selectedSkill = skillTreeTemplates[selectedSkillTreeIndex];
                string assetPath = AssetDatabase.GetAssetPath(selectedSkill);
                skillTreeTemplates.RemoveAt(selectedSkillTreeIndex);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        private void LoadSkillTreeTemplates()
        {
            skillTreeTemplates.Clear();
            string[] guids = AssetDatabase.FindAssets("t:SkillTreeGraph");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SkillTreeGraph skillTree = AssetDatabase.LoadAssetAtPath<SkillTreeGraph>(path);

                skillTreeTemplates.Add(skillTree);
            }
        }
        #endregion

        #region ��ƿ��Ƽ
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
