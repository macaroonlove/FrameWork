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
    public class DataSystemEditorWindow : EditorWindow
    {
        private int selectedTab = 0;
        private Texture2D emptyTexture2D;

        private Vector2 contentScrollPosition;

        #region 아군 유닛
        private int selectedAgentIndex = 0;
        private Vector2 agentScrollPosition;
        private List<Tuple<AgentTemplate, Texture2D>> agentTemplates = new List<Tuple<AgentTemplate, Texture2D>>();
        #endregion
        #region 적군 유닛
        private int selectedEnemyIndex = 0;
        private Vector2 enemyScrollPosition;
        private List<Tuple<EnemyTemplate, Texture2D>> enemyTemplates = new List<Tuple<EnemyTemplate, Texture2D>>();
        #endregion
        #region 버프
        private int selectedBuffIndex = 0;
        private Vector2 buffScrollPosition;
        private List<BuffTemplate> buffTemplates = new List<BuffTemplate>();
        #endregion
        #region 상태이상
        private int selectedAbnormalStatusIndex = 0;
        private Vector2 abnormalStatusScrollPosition;
        private List<Tuple<AbnormalStatusTemplate, Texture2D>> abnormalStatusTemplates = new List<Tuple<AbnormalStatusTemplate, Texture2D>>();
        #endregion
        #region 스킬
        private int selectedSkillIndex = 0;
        private Vector2 skillScrollPosition;
        private List<Tuple<SkillTemplate, Texture2D>> skillTemplates = new List<Tuple<SkillTemplate, Texture2D>>();
        #endregion
        #region 스킬 트리
        private int selectedSkillTreeIndex = 0;
        private Vector2 skillTreeScrollPosition;
        private List<SkillTreeGraph> skillTreeTemplates = new List<SkillTreeGraph>();
        #endregion

        [MenuItem("Window/게임 데이터 관리 시스템")]
        public static void Open()
        {
            var window = GetWindow<DataSystemEditorWindow>();
            window.titleContent = new GUIContent("게임 데이터 관리");
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
            if (GUILayout.Toggle(selectedTab == 0, "아군 유닛", "Button")) selectedTab = 0;
            if (GUILayout.Toggle(selectedTab == 1, "적 유닛", "Button")) selectedTab = 1;
            if (GUILayout.Toggle(selectedTab == 2, "버프", "Button")) selectedTab = 2;
            if (GUILayout.Toggle(selectedTab == 3, "상태이상", "Button")) selectedTab = 3;
            if (GUILayout.Toggle(selectedTab == 4, "엑티브 스킬", "Button")) selectedTab = 4;
            if (GUILayout.Toggle(selectedTab == 5, "스킬트리", "Button")) selectedTab = 5;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            switch (selectedTab)
            {
                case 0:
                    DrawAgentTab();
                    break;
                case 1:
                    DrawEnemyTab();
                    break;
                case 2:
                    DrawBuffTab();
                    break;
                case 3:
                    DrawAbnormalStatusTab();
                    break;
                case 4:
                    DrawActiveSkillTab();
                    break;
                case 5:
                    DrawSkillTreeTab();
                    break;
            }
        }

        #region 아군 유닛
        private void DrawAgentTab()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("아군 유닛 추가"))
            {
                AddAgentTemplate();
            }
            if (GUILayout.Button("아군 유닛 삭제"))
            {
                DeleteSelectedAgentTemplate();
            }
            if (GUILayout.Button("아군 유닛 탐색"))
            {
                LoadAgentTemplates();
            }

            DrawLine();

            agentScrollPosition = GUILayout.BeginScrollView(agentScrollPosition, false, true);

            var agentCatalog = new GUIStyle(GUI.skin.button);
            agentCatalog.alignment = TextAnchor.MiddleLeft;
            agentCatalog.padding = new RectOffset(5, 5, 5, 5);
            agentCatalog.margin = new RectOffset(5, 5, -2, -2);
            agentCatalog.border = new RectOffset(0, 0, 0, 0);
            agentCatalog.fixedWidth = GUI.skin.box.fixedWidth;
            agentCatalog.fixedHeight = GUI.skin.box.fixedHeight;

            for (int i = 0; i < agentTemplates.Count; i++)
            {
                bool isSelected = (selectedAgentIndex == i);

                var text = "  " + agentTemplates[i].Item1.displayName;
                text = text.Substring(0, Mathf.Min(text.Length, 13));
                GUIContent content = new GUIContent(text, agentTemplates[i].Item2);

                if (GUILayout.Toggle(isSelected, content, agentCatalog))
                {
                    if (selectedAgentIndex != i)
                    {
                        selectedAgentIndex = i;

                        GUI.FocusControl(null);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (agentTemplates.Count > 0 && selectedAgentIndex < agentTemplates.Count)
            {
                AgentTemplate selectedAgent = agentTemplates[selectedAgentIndex].Item1;

                contentScrollPosition = GUILayout.BeginScrollView(contentScrollPosition, false, false);
                var editor = UnityEditor.Editor.CreateEditor(selectedAgent);
                editor.OnInspectorGUI();
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void AddAgentTemplate()
        {
            // 아군 유닛 템플릿 생성
            AgentTemplate newAgent = CreateInstance<AgentTemplate>();

            // 에셋 저장
            string defaultPath = "Assets/FrameWork/Core/GameData/Agent";
            string path = EditorUtility.SaveFilePanelInProject("FrameWork/Core/GameData/Agent", "Agent_", "asset", "아군 템플릿은 FrameWork/Core/GameData/Agent 위치에 저장됩니다..", defaultPath);
            if (!string.IsNullOrEmpty(path))
            {
                newAgent.SetDisplayName(Path.GetFileNameWithoutExtension(path));

                AssetDatabase.CreateAsset(newAgent, path);
                AssetDatabase.SaveAssets();
                LoadAgentTemplates();
            }
        }

        private void DeleteSelectedAgentTemplate()
        {
            if (agentTemplates.Count > 0)
            {
                AgentTemplate selectedAgent = agentTemplates[selectedAgentIndex].Item1;
                string assetPath = AssetDatabase.GetAssetPath(selectedAgent);
                agentTemplates.RemoveAt(selectedAgentIndex);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        private void LoadAgentTemplates()
        {
            if (emptyTexture2D == null) emptyTexture2D = CreateTexture(Color.gray);

            agentTemplates.Clear();
            string[] guids = AssetDatabase.FindAssets("t:AgentTemplate");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AgentTemplate agent = AssetDatabase.LoadAssetAtPath<AgentTemplate>(path);

                var texture = (agent.sprite == null) ? emptyTexture2D : agent.sprite.texture;
                texture = texture.ResizeTexture(30, 30);

                agentTemplates.Add(new Tuple<AgentTemplate, Texture2D>(agent, texture));
            }
        }
        #endregion

        #region 적 유닛
        private void DrawEnemyTab()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("적 유닛 추가"))
            {
                AddEnemyTemplate();
            }
            if (GUILayout.Button("적 유닛 삭제"))
            {
                DeleteSelectedEnemyTemplate();
            }
            if (GUILayout.Button("적 유닛 탐색"))
            {
                LoadEnemyTemplates();
            }

            DrawLine();

            agentScrollPosition = GUILayout.BeginScrollView(agentScrollPosition, false, true);

            var enemyCatalog = new GUIStyle(GUI.skin.button);
            enemyCatalog.alignment = TextAnchor.MiddleLeft;
            enemyCatalog.padding = new RectOffset(5, 5, 5, 5);
            enemyCatalog.margin = new RectOffset(5, 5, -2, -2);
            enemyCatalog.border = new RectOffset(0, 0, 0, 0);
            enemyCatalog.fixedWidth = GUI.skin.box.fixedWidth;
            enemyCatalog.fixedHeight = GUI.skin.box.fixedHeight;

            for (int i = 0; i < enemyTemplates.Count; i++)
            {
                bool isSelected = (selectedEnemyIndex == i);

                var text = "  " + enemyTemplates[i].Item1.displayName;
                text = text.Substring(0, Mathf.Min(text.Length, 13));
                GUIContent content = new GUIContent(text, enemyTemplates[i].Item2);

                if (GUILayout.Toggle(isSelected, content, enemyCatalog))
                {
                    if (selectedEnemyIndex != i)
                    {
                        selectedEnemyIndex = i;

                        GUI.FocusControl(null);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (enemyTemplates.Count > 0 && selectedEnemyIndex < enemyTemplates.Count)
            {
                EnemyTemplate selectedEnemy = enemyTemplates[selectedEnemyIndex].Item1;

                contentScrollPosition = GUILayout.BeginScrollView(contentScrollPosition, false, false);
                var editor = UnityEditor.Editor.CreateEditor(selectedEnemy);
                editor.OnInspectorGUI();
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void AddEnemyTemplate()
        {
            // 적 유닛 템플릿 생성
            EnemyTemplate newEnemy = CreateInstance<EnemyTemplate>();

            // 에셋 저장
            string defaultPath = "Assets/FrameWork/Core/GameData/Enemy";
            string path = EditorUtility.SaveFilePanelInProject("FrameWork/Core/GameData/Enemy", "Enemy_", "asset", "적 템플릿은 FrameWork/Core/GameData/Enemy 위치에 저장됩니다..", defaultPath);
            if (!string.IsNullOrEmpty(path))
            {
                newEnemy.SetDisplayName(Path.GetFileNameWithoutExtension(path));

                AssetDatabase.CreateAsset(newEnemy, path);
                AssetDatabase.SaveAssets();
                LoadEnemyTemplates();
            }
        }

        private void DeleteSelectedEnemyTemplate()
        {
            if (enemyTemplates.Count > 0)
            {
                EnemyTemplate selectedEnemy = enemyTemplates[selectedEnemyIndex].Item1;
                string assetPath = AssetDatabase.GetAssetPath(selectedEnemy);
                enemyTemplates.RemoveAt(selectedEnemyIndex);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        private void LoadEnemyTemplates()
        {
            if (emptyTexture2D == null) emptyTexture2D = CreateTexture(Color.gray);

            enemyTemplates.Clear();
            string[] guids = AssetDatabase.FindAssets("t:EnemyTemplate");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EnemyTemplate enemy = AssetDatabase.LoadAssetAtPath<EnemyTemplate>(path);

                var texture = (enemy.sprite == null) ? emptyTexture2D : enemy.sprite.texture;
                texture = texture.ResizeTexture(30, 30);

                enemyTemplates.Add(new Tuple<EnemyTemplate, Texture2D>(enemy, texture));
            }
        }
        #endregion

        #region 버프
        private void DrawBuffTab()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("버프 추가"))
            {
                AddBuffTemplate();
            }
            if (GUILayout.Button("버프 삭제"))
            {
                DeleteSelectedBuffTemplate();
            }
            if (GUILayout.Button("버프 탐색"))
            {
                LoadBuffTemplates();
            }

            DrawLine();

            buffScrollPosition = GUILayout.BeginScrollView(buffScrollPosition, false, true);

            var buffCatalog = new GUIStyle(GUI.skin.button);
            buffCatalog.alignment = TextAnchor.MiddleLeft;
            buffCatalog.padding = new RectOffset(5, 5, 5, 5);
            buffCatalog.margin = new RectOffset(5, 5, -2, -2);
            buffCatalog.border = new RectOffset(0, 0, 0, 0);
            buffCatalog.fixedWidth = GUI.skin.box.fixedWidth;
            buffCatalog.fixedHeight = GUI.skin.box.fixedHeight;

            for (int i = 0; i < buffTemplates.Count; i++)
            {
                bool isSelected = (selectedBuffIndex == i);

                var text = "  " + buffTemplates[i].displayName;
                text = text.Substring(0, Mathf.Min(text.Length, 13));

                if (GUILayout.Toggle(isSelected, text, buffCatalog))
                {
                    if (selectedBuffIndex != i)
                    {
                        selectedBuffIndex = i;

                        GUI.FocusControl(null);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (buffTemplates.Count > 0 && selectedBuffIndex < buffTemplates.Count)
            {
                BuffTemplate selectedbuff = buffTemplates[selectedBuffIndex];

                contentScrollPosition = GUILayout.BeginScrollView(contentScrollPosition, false, false);
                var editor = UnityEditor.Editor.CreateEditor(selectedbuff);
                editor.OnInspectorGUI();
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void AddBuffTemplate()
        {
            // 버프 템플릿 생성
            BuffTemplate newBuff = CreateInstance<BuffTemplate>();

            // 에셋 저장
            string defaultPath = "Assets/FrameWork/Core/GameData/Status/BuffStatus";
            string path = EditorUtility.SaveFilePanelInProject("FrameWork/Core/GameData/Status/BuffStatus", "BuffStatus_", "asset", "상태이상 템플릿은 FrameWork/Core/GameData/Status/BuffStatus 위치에 저장됩니다..", defaultPath);
            if (!string.IsNullOrEmpty(path))
            {
                newBuff.SetDisplayName(Path.GetFileNameWithoutExtension(path));

                AssetDatabase.CreateAsset(newBuff, path);
                AssetDatabase.SaveAssets();
                LoadBuffTemplates();
            }
        }

        private void DeleteSelectedBuffTemplate()
        {
            if (buffTemplates.Count > 0)
            {
                BuffTemplate selectedBuff = buffTemplates[selectedBuffIndex];
                string assetPath = AssetDatabase.GetAssetPath(selectedBuff);
                buffTemplates.RemoveAt(selectedBuffIndex);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        private void LoadBuffTemplates()
        {
            buffTemplates.Clear();
            string[] guids = AssetDatabase.FindAssets("t:BuffTemplate");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BuffTemplate buff = AssetDatabase.LoadAssetAtPath<BuffTemplate>(path);

                buffTemplates.Add(buff);
            }
        }
        #endregion

        #region 상태이상
        private void DrawAbnormalStatusTab()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("상태이상 추가"))
            {
                AddAbnormalStatusTemplate();
            }
            if (GUILayout.Button("상태이상 삭제"))
            {
                DeleteSelectedAbnormalStatusTemplate();
            }
            if (GUILayout.Button("상태이상 탐색"))
            {
                LoadAbnormalStatusTemplates();
            }

            DrawLine();

            abnormalStatusScrollPosition = GUILayout.BeginScrollView(abnormalStatusScrollPosition, false, true);

            var abnormalStatusCatalog = new GUIStyle(GUI.skin.button);
            abnormalStatusCatalog.alignment = TextAnchor.MiddleLeft;
            abnormalStatusCatalog.padding = new RectOffset(5, 5, 5, 5);
            abnormalStatusCatalog.margin = new RectOffset(5, 5, -2, -2);
            abnormalStatusCatalog.border = new RectOffset(0, 0, 0, 0);
            abnormalStatusCatalog.fixedWidth = GUI.skin.box.fixedWidth;
            abnormalStatusCatalog.fixedHeight = GUI.skin.box.fixedHeight;

            for (int i = 0; i < abnormalStatusTemplates.Count; i++)
            {
                bool isSelected = (selectedAbnormalStatusIndex == i);

                var text = "  " + abnormalStatusTemplates[i].Item1.displayName;
                text = text.Substring(0, Mathf.Min(text.Length, 13));
                GUIContent content = new GUIContent(text, abnormalStatusTemplates[i].Item2);

                if (GUILayout.Toggle(isSelected, content, abnormalStatusCatalog))
                {
                    if (selectedAbnormalStatusIndex != i)
                    {
                        selectedAbnormalStatusIndex = i;

                        GUI.FocusControl(null);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (abnormalStatusTemplates.Count > 0 && selectedAbnormalStatusIndex < abnormalStatusTemplates.Count)
            {
                AbnormalStatusTemplate selectedabnormalStatus = abnormalStatusTemplates[selectedAbnormalStatusIndex].Item1;

                contentScrollPosition = GUILayout.BeginScrollView(contentScrollPosition, false, false);
                var editor = UnityEditor.Editor.CreateEditor(selectedabnormalStatus);
                editor.OnInspectorGUI();
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void AddAbnormalStatusTemplate()
        {
            // 상태이상 템플릿 생성
            AbnormalStatusTemplate newAbnormalStatus = CreateInstance<AbnormalStatusTemplate>();

            // 에셋 저장
            string defaultPath = "Assets/FrameWork/Core/GameData/Status/AbnormalStatus";
            string path = EditorUtility.SaveFilePanelInProject("FrameWork/Core/GameData/Status/AbnormalStatus", "AbnormalStatus_", "asset", "상태이상 템플릿은 FrameWork/Core/GameData/Status/AbnormalStatus 위치에 저장됩니다..", defaultPath);
            if (!string.IsNullOrEmpty(path))
            {
                newAbnormalStatus.SetDisplayName(Path.GetFileNameWithoutExtension(path));

                AssetDatabase.CreateAsset(newAbnormalStatus, path);
                AssetDatabase.SaveAssets();
                LoadAbnormalStatusTemplates();
            }
        }

        private void DeleteSelectedAbnormalStatusTemplate()
        {
            if (abnormalStatusTemplates.Count > 0)
            {
                AbnormalStatusTemplate selectedAbnormalStatus = abnormalStatusTemplates[selectedAbnormalStatusIndex].Item1;
                string assetPath = AssetDatabase.GetAssetPath(selectedAbnormalStatus);
                abnormalStatusTemplates.RemoveAt(selectedAbnormalStatusIndex);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        private void LoadAbnormalStatusTemplates()
        {
            if (emptyTexture2D == null) emptyTexture2D = CreateTexture(Color.gray);

            abnormalStatusTemplates.Clear();
            string[] guids = AssetDatabase.FindAssets("t:AbnormalStatusTemplate");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AbnormalStatusTemplate abnormalStatus = AssetDatabase.LoadAssetAtPath<AbnormalStatusTemplate>(path);

                var texture = (abnormalStatus.sprite == null) ? emptyTexture2D : abnormalStatus.sprite.texture;
                texture = texture.ResizeTexture(30, 30);

                abnormalStatusTemplates.Add(new Tuple<AbnormalStatusTemplate, Texture2D>(abnormalStatus, texture));
            }
        }
        #endregion

        #region 스킬
        private void DrawActiveSkillTab()
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
                    if (selectedSkillIndex != i)
                    {
                        selectedSkillIndex = i;

                        GUI.FocusControl(null);
                    }
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
                newSkill.SetDisplayName(Path.GetFileNameWithoutExtension(path));

                contentScrollPosition = GUILayout.BeginScrollView(contentScrollPosition, false, false);
                AssetDatabase.CreateAsset(newSkill, path);
                AssetDatabase.SaveAssets();
                LoadSkillTemplates();
                GUILayout.EndScrollView();
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
            if (emptyTexture2D == null) emptyTexture2D = CreateTexture(Color.gray);

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

        #region 스킬트리
        private void DrawSkillTreeTab()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("스킬 트리 추가"))
            {
                AddSkillTreeTemplate();
            }
            if (GUILayout.Button("스킬 트리 삭제"))
            {
                DeleteSelectedSkillTreeTemplate();
            }
            if (GUILayout.Button("스킬 트리 탐색"))
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

            if (GUILayout.Button("에디터 열기"))
            {
                NodeEditorWindow.Open(skillTreeTemplates[selectedSkillTreeIndex]);
            }
            
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void AddSkillTreeTemplate()
        {
            // 스킬 템플릿 생성
            SkillTreeGraph newSkill = CreateInstance<SkillTreeGraph>();

            // 에셋 저장
            string defaultPath = "Assets/FrameWork/Core/GameData/SkillTree";
            string path = EditorUtility.SaveFilePanelInProject("FrameWork/Core/GameData/SkillTree", "SkillTree_", "asset", "스킬 트리 템플릿을 저장합니다.", defaultPath);
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
