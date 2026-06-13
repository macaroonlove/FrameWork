//using PrimeTween;
//using UnityEngine;
//using System.Collections.Generic;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//namespace FrameWork.TweenExtension
//{
//    [System.Serializable]
//    public class NGUISpriteAnimationTweenCommand : TweenCommand
//    {
//        public UISprite target;

//        public string spritePrefix = "";
//        public string[] animationFrames;

//        public float duration = 1f;
//        public Ease ease = Ease.Linear;

//        [HideInInspector] public string _startSpriteName;
//        [HideInInspector] public bool _isBaked = false;

//        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1f, float startDelay = 0f)
//        {
//            if (target == null || animationFrames == null || animationFrames.Length == 0) return;

//            float finalDuration = duration / timeScale;
//            string[] capturedFrames = animationFrames;

//            Tween tween = Tween.Custom(target, 0f, 0.999f, finalDuration, delegate (UISprite t, float val)
//            {
//                int frameIndex = Mathf.FloorToInt(val * capturedFrames.Length);
//                t.spriteName = capturedFrames[frameIndex];
//            }, ease, startDelay: startDelay);

//            if (isJoinMode) sequence.Group(tween);
//            else sequence.Chain(tween);
//        }

//        public override void RecordStartValues(bool isForce)
//        {
//            if (target == null) return;

//            if (isForce || !_isBaked)
//            {
//                _startSpriteName = target.spriteName;
//                _isBaked = true;
//            }
//        }

//        public override void RestoreStartValues()
//        {
//            if (target != null && _isBaked && !string.IsNullOrEmpty(_startSpriteName))
//            {
//                target.spriteName = _startSpriteName;
//            }
//        }

//#if UNITY_EDITOR
//        public override string GetDescription() => "NGUI 스프라이트 전환 (Animation)";

//        public override void Draw(Rect rect)
//        {
//            float h = EditorGUIUtility.singleLineHeight;
//            float currentY = rect.y;

//            GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel);
//            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.2f, 0.2f);
//            SerializedProperty prop = currentDrawingProperty;

//            // 1. 타겟 설정
//            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "타겟 (UISprite) 설정", headerStyle);
//            currentY += h + 2;

//            EditorGUI.BeginChangeCheck();
//            target = (UISprite)EditorGUI.ObjectField(new Rect(rect.x, currentY, rect.width, h), "Target", target, typeof(UISprite), true);
//            if (EditorGUI.EndChangeCheck()) _isBaked = false;
//            currentY += h + 5;

//            DrawSeparator(rect.x, currentY, rect.width);
//            currentY += 6;

//            // 2. 애니메이션 프레임 및 접두사 검색
//            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "프레임 시퀀스 수집 (접두사 검색)", headerStyle);
//            currentY += h + 2;

//            // 접두사 입력 필드와 버튼을 한 줄에 배치
//            float halfWidth = (rect.width - 10) / 2f;
//            spritePrefix = EditorGUI.TextField(new Rect(rect.x, currentY, halfWidth, h), "접두사 (Prefix)", spritePrefix);

//            if (GUI.Button(new Rect(rect.x + halfWidth + 10, currentY, halfWidth, h), "아틀라스에서 프레임 찾기"))
//            {
//                if (target != null && target.atlas != null)
//                {
//                    // NGUI Atlas에서 스프라이트 리스트 가져오기
//                    var sprites = target.atlas.spriteList;
//                    List<string> matchedNames = new List<string>();

//                    for (int i = 0; i < sprites.Count; i++)
//                    {
//                        if (sprites[i].name.StartsWith(spritePrefix))
//                        {
//                            matchedNames.Add(sprites[i].name);
//                        }
//                    }

//                    // 알파벳/숫자 순서대로 정렬하여 프레임 순서 보장
//                    matchedNames.Sort();
//                    animationFrames = matchedNames.ToArray();

//                    Debug.Log($"[TweenExecutor] '{spritePrefix}' 접두사를 가진 {matchedNames.Count}개의 프레임을 성공적으로 찾았습니다!");
//                }
//                else
//                {
//                    Debug.LogWarning("[TweenExecutor] 타겟이 없거나 타겟에 Atlas가 할당되어 있지 않습니다.");
//                }
//            }
//            currentY += h + 2;

//            // 추출된 배열 그리기
//            if (prop != null)
//            {
//                SerializedProperty framesProp = prop.FindPropertyRelative("animationFrames");
//                if (framesProp != null)
//                {
//                    float height = EditorGUI.GetPropertyHeight(framesProp, true);
//                    EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, height), framesProp, true);
//                    currentY += height + 5;
//                }
//            }

//            DrawSeparator(rect.x, currentY, rect.width);
//            currentY += 6;

//            // 3. 시간 및 곡선
//            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "재생 설정", headerStyle);
//            currentY += h + 2;

//            duration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "지속 시간 (Duration)", duration);
//            currentY += h + 2;
//            ease = (Ease)EditorGUI.EnumPopup(new Rect(rect.x, currentY, rect.width, h), "재생 방식 (Ease)", ease);
//        }

//        public override int GetNumRows()
//        {
//            if (isFolded) return 0;
//            int rows = 2; // 기본 패딩
//            SerializedProperty prop = currentDrawingProperty;

//            rows += 1; // target 필드
//            rows += 1; // 프레임 타이틀
//            rows += 1; // 접두사 검색 라인 (TextField + Button)

//            // Frames 배열 높이
//            if (prop != null)
//            {
//                SerializedProperty framesProp = prop.FindPropertyRelative("animationFrames");
//                if (framesProp != null) rows += Mathf.CeilToInt(EditorGUI.GetPropertyHeight(framesProp, true) / (EditorGUIUtility.singleLineHeight + 2));
//            }
//            else rows += 1;

//            rows += 1; // 재생 설정 타이틀
//            rows += 2; // duration, ease

//            return rows;
//        }
//#endif
//    }
//}