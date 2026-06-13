#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FrameWork.TweenExtension.Editor
{
    [CustomEditor(typeof(TweenExecutor))]
    public class TweenExecutorEditor : UnityEditor.Editor
    {
        private TweenExecutor _target;
        private ReorderableList _commandsList;

        private void OnEnable()
        {
            _target = (TweenExecutor)target;
            EditorApplication.update += OnEditorUpdate;

            SerializedProperty commandsProp = serializedObject.FindProperty("commands");
            _commandsList = new ReorderableList(serializedObject, commandsProp, true, true, true, true);

            _commandsList.drawHeaderCallback = (rect) =>
            {
                float buttonWidth = 65f;
                Rect titleRect = new Rect(rect.x, rect.y, rect.width - (buttonWidth * 2 + 10), rect.height);
                EditorGUI.LabelField(titleRect, "Maca Tween Timeline", EditorStyles.boldLabel);

                Rect foldAllRect = new Rect(rect.x + rect.width - (buttonWidth * 2 + 5), rect.y + 1, buttonWidth, rect.height - 3);
                if (GUI.Button(foldAllRect, "모두 접기", EditorStyles.miniButtonLeft))
                {
                    Undo.RecordObject(_target, "Fold All");
                    foreach (var cmd in _target.commands) if (cmd != null) cmd.isFolded = true;
                }

                Rect unfoldAllRect = new Rect(rect.x + rect.width - buttonWidth, rect.y + 1, buttonWidth, rect.height - 3);
                if (GUI.Button(unfoldAllRect, "모두 펴기", EditorStyles.miniButtonRight))
                {
                    Undo.RecordObject(_target, "Unfold All");
                    foreach (var cmd in _target.commands) if (cmd != null) cmd.isFolded = false;
                }
            };

            _commandsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _target.commands[index];
                if (element == null) return;

                TweenCommand.currentDrawingProperty = _commandsList.serializedProperty.GetArrayElementAtIndex(index);

                Color blockColor = new Color(0.25f, 0.25f, 0.25f, 0.4f);
                Color accentColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

                string typeName = element.GetType().Name;

                // 기본값
                blockColor = new Color(0.2f, 0.2f, 0.2f, 0.3f);
                accentColor = new Color(0.6f, 0.6f, 0.6f, 0.9f);

                if (typeName.Contains("Position")) { blockColor = new Color(0.15f, 0.3f, 0.45f, 0.25f); accentColor = new Color(0.2f, 0.5f, 0.8f, 0.9f); }
                else if (typeName.Contains("Rotation")) { blockColor = new Color(0.45f, 0.2f, 0.2f, 0.25f); accentColor = new Color(0.8f, 0.3f, 0.3f, 0.9f); }
                else if (typeName.Contains("Scale")) { blockColor = new Color(0.2f, 0.4f, 0.2f, 0.25f); accentColor = new Color(0.3f, 0.7f, 0.3f, 0.9f); }
                else if (typeName.Contains("Delay")) { blockColor = new Color(0.4f, 0.35f, 0.15f, 0.25f); accentColor = new Color(0.8f, 0.6f, 0.2f, 0.9f); }
                else if (typeName.Contains("Join")) { blockColor = new Color(0.35f, 0.15f, 0.4f, 0.25f); accentColor = new Color(0.6f, 0.3f, 0.8f, 0.9f); }
                // 컬러 계열
                else if (typeName.Contains("Color")) { blockColor = new Color(0.4f, 0.25f, 0.1f, 0.25f); accentColor = new Color(0.9f, 0.6f, 0.2f, 0.9f); }
                // 알파 계열
                else if (typeName.Contains("Alpha")) { blockColor = new Color(0.1f, 0.35f, 0.35f, 0.25f); accentColor = new Color(0.2f, 0.8f, 0.8f, 0.9f); }
                // 애니메이션 계열
                else if (typeName.Contains("Animation")) { blockColor = new Color(0.4f, 0.1f, 0.3f, 0.25f); accentColor = new Color(1.0f, 0.4f, 0.7f, 0.9f); }
                // 시스템 이벤트 계열
                else if (typeName.Contains("Active") || typeName.Contains("Particle")) { blockColor = new Color(0.2f, 0.2f, 0.2f, 0.3f); accentColor = new Color(0.7f, 0.7f, 0.7f, 0.9f); }

                if (isActive || isFocused) blockColor.a += 0.15f;

                Rect boxRect = new Rect(rect.x - 14, rect.y + 2, rect.width + 18, rect.height - 4);
                EditorGUI.DrawRect(boxRect, blockColor);
                DrawRectOutline(boxRect, new Color(accentColor.r, accentColor.g, accentColor.b, 0.25f));
                EditorGUI.DrawRect(new Rect(boxRect.x, boxRect.y, 4, boxRect.height), accentColor);

                float originalX = rect.x;
                rect.x += 8; rect.width -= 12; rect.y += 6; rect.height = EditorGUIUtility.singleLineHeight;

                GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
                labelStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                labelStyle.fontSize = 11;

                Rect foldoutRect = new Rect(rect.x, rect.y, 15, rect.height);
                Rect labelRect = new Rect(rect.x + 15, rect.y, rect.width - 15, rect.height);

                EditorGUI.BeginChangeCheck();
                element.isFolded = !EditorGUI.Foldout(foldoutRect, !element.isFolded, GUIContent.none, true);
                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(_target);

                string text = $"[{index}] {element.GetDescription()}";

                GUIStyle style = EditorStyles.label;

                Vector2 size = style.CalcSize(new GUIContent(text));

                while (size.x > labelRect.width && text.Length > 3)
                {
                    text = text.Substring(0, text.Length - 4) + "...";
                    size = style.CalcSize(new GUIContent(text));
                }

                if (GUI.Button(labelRect, text, style))
                {
                    Undo.RecordObject(_target, "Toggle Fold");
                    element.isFolded = !element.isFolded;
                    EditorUtility.SetDirty(_target);
                }

                if (element is IColorTweenCommand colorCommand)
                {
                    Rect colorRect = new Rect(
                        labelRect.xMax - 20,
                        labelRect.y + 1,
                        16,
                        16);

                    if (colorCommand.IsRandomEndColor)
                    {
                        Rect left = colorRect;
                        left.width *= 0.5f;

                        Rect right = colorRect;
                        right.x += right.width * 0.5f;
                        right.width *= 0.5f;

                        EditorGUI.DrawRect(left, colorCommand.MinEndColor);
                        EditorGUI.DrawRect(right, colorCommand.MaxEndColor);
                    }
                    else
                    {
                        EditorGUI.DrawRect(colorRect, colorCommand.MinEndColor);
                    }
                }

                if (!element.isFolded)
                {
                    rect.y += EditorGUIUtility.singleLineHeight + 6;
                    EditorGUI.BeginChangeCheck();
                    element.Draw(rect);
                    if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(_target);
                }

                rect.x = originalX;
            };

            _commandsList.elementHeightCallback = (index) =>
            {
                var element = _target.commands[index];
                if (element == null) return EditorGUIUtility.singleLineHeight;
                TweenCommand.currentDrawingProperty = _commandsList.serializedProperty.GetArrayElementAtIndex(index);
                if (element.isFolded) return EditorGUIUtility.singleLineHeight + 12;
                return EditorGUIUtility.singleLineHeight + 12 + element.GetHeight() + 8;
            };

            _commandsList.onAddDropdownCallback = (buttonRect, list) =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("이동 (Position)"), false, () => AddCommand(new PositionTweenCommand()));
                menu.AddItem(new GUIContent("회전 (Rotation)"), false, () => AddCommand(new RotationTweenCommand()));
                menu.AddItem(new GUIContent("크기 (Scale)"), false, () => AddCommand(new ScaleTweenCommand()));
                menu.AddSeparator("");

                menu.AddItem(new GUIContent("오브젝트 활성화 (Active)"), false, () => AddCommand(new ActiveTweenCommand()));
                menu.AddItem(new GUIContent("파티클 이미션 제어 (Particle Emission)"), false, () => AddCommand(new ParticleEmissionTweenCommand()));
                menu.AddSeparator("");

                menu.AddItem(new GUIContent("머티리얼 색상"), false, () => AddCommand(new MaterialColorTweenCommand()));
                menu.AddItem(new GUIContent("머티리얼 투명도"), false, () => AddCommand(new MaterialAlphaTweenCommand()));
                menu.AddSeparator("");

                menu.AddItem(new GUIContent("스프라이트 색상"), false, () => AddCommand(new SpriteColorTweenCommand()));
                menu.AddItem(new GUIContent("스프라이트 투명도)"), false, () => AddCommand(new SpriteAlphaTweenCommand()));
                menu.AddItem(new GUIContent("스프라이트 애니메이션"), false, () => AddCommand(new SpriteAnimationTweenCommand()));
                menu.AddSeparator("");

                menu.AddItem(new GUIContent("UGUI 색상"), false, () => AddCommand(new UGUIColorTweenCommand()));
                menu.AddItem(new GUIContent("UGUI 투명도"), false, () => AddCommand(new UGUIAlphaTweenCommand()));
                menu.AddItem(new GUIContent("UGUI 애니메이션"), false, () => AddCommand(new UGUIAnimationTweenCommand()));
                menu.AddSeparator("");

                //menu.AddItem(new GUIContent("NGUI 색상"), false, () => AddCommand(new NGUISpriteColorTweenCommand()));
                //menu.AddItem(new GUIContent("NGUI 투명도"), false, () => AddCommand(new NGUISpriteAlphaTweenCommand()));
                //menu.AddItem(new GUIContent("NGUI 애니메이션"), false, () => AddCommand(new NGUISpriteAnimationTweenCommand()));
                //menu.AddSeparator("");

                menu.AddItem(new GUIContent("대기 (Delay)"), false, () => AddCommand(new DelayTweenCommand()));
                menu.AddItem(new GUIContent("동시 실행 (Join)"), false, () => AddCommand(new JoinTweenCommand()));
                menu.ShowAsContext();
            };
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void AddCommand(TweenCommand newCommand)
        {
            Undo.RecordObject(_target, "Add Tween Command");
            newCommand.isFolded = false;
            _target.commands.Add(newCommand);
            EditorUtility.SetDirty(_target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // --- 재생 공통 설정 ---
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("시스템 설정", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playOnEnable"), new GUIContent("활성화 시 자동 재생"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("restoreOnStop"), new GUIContent("정지 시 초기값 복구"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeScale"), new GUIContent("전체 배속"));

            SerializedProperty isLoopProp = serializedObject.FindProperty("isLoop");
            EditorGUILayout.PropertyField(isLoopProp, new GUIContent("무한 루프 반복"));
            if (!isLoopProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cycleCount"), new GUIContent("반복 횟수"));
                EditorGUI.indentLevel--;
            }

            // --- 타임라인 리스트 ---
            EditorGUILayout.Space();
            _commandsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            // --- 데이터 베이킹 및 컨트롤러 ---
            GUILayout.Space(15);

            GUI.backgroundColor = new Color(0.7f, 0.9f, 0.7f);
            if (GUILayout.Button("현재 상태를 시작값으로 적용 (Bake)", GUILayout.Height(30)))
            {
                Undo.RecordObject(_target, "Record Start Values");
                _target.RecordStartValues(true);
                EditorUtility.SetDirty(_target);
                Debug.Log("[TweenExecutor] 타임라인에 등록된 오브젝트들의 현재 트랜스폼 값이 시작값으로 저장되었습니다.");
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(5);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            GUILayout.Space(5);

            float buttonHalfWidth = (EditorGUIUtility.currentViewWidth - 40f) / 2f;

            GUILayout.BeginHorizontal();
            {
                GUI.backgroundColor = _target.IsScrubbing ? Color.yellow : Color.white;
                if (GUILayout.Button(_target.IsScrubbing ? "⏹ 스크럽 종료" : "🔍 스크럽 시작", GUILayout.Width(buttonHalfWidth), GUILayout.Height(30)))
                {
                    _target.RecordStartValues(false);
                    _target.ToggleScrubMode(!_target.IsScrubbing);
                }
                GUI.backgroundColor = Color.white;

                if (_target.IsSequenceAlive() && !_target.IsScrubbing)
                {
                    if (GUILayout.Button("⏸ 정지", GUILayout.Width(buttonHalfWidth), GUILayout.Height(30)))
                    {
                        _target.StopAllTweens();
                    }
                }
                else
                {
                    if (GUILayout.Button("▶ 재생", GUILayout.Width(buttonHalfWidth), GUILayout.Height(30)))
                    {
                        _target.RecordStartValues(false);
                        _target.Play();
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("🔄 초기값 복구", GUILayout.Width(buttonHalfWidth), GUILayout.Height(30)))
                {
                    _target.RecordStartValues(false);
                    _target.StopAllTweens();
                    _target.RestoreStartValues();
                }
                if (GUILayout.Button("⏩ 즉시 완료", GUILayout.Width(buttonHalfWidth), GUILayout.Height(30)))
                {
                    _target.RecordStartValues(false);
                    _target.CompleteTweens();
                }
            }
            GUILayout.EndHorizontal();

            // 스크럽 슬라이더
            if (_target.IsScrubbing && _target.IsSequenceAlive())
            {
                GUILayout.Space(10);
                EditorGUI.BeginChangeCheck();
                float p = EditorGUILayout.Slider("스크럽 타임라인", _target.GetSequenceProgress(), 0f, 1f);
                if (EditorGUI.EndChangeCheck())
                {
                    _target.SetSequenceProgress(p);
                    SceneView.RepaintAll();
                }
            }
        }

        // 스크럽 중일 때 씬뷰가 부드럽게 업데이트되도록 훅 추가
        public override bool RequiresConstantRepaint()
        {
            return _target != null && _target.IsSequenceAlive();
        }

        void OnEditorUpdate()
        {
            if (_target != null && _target.IsSequenceAlive() && !_target.IsScrubbing)
            {
                Repaint(); // 재생 중엔 인스펙터를 갱신하여 씬과 동기화
            }
        }

        private void DrawRectOutline(Rect rect, Color color)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - 1, rect.width, 1), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.x + rect.width - 1, rect.y, 1, rect.height), color);
        }
    }
}
#endif