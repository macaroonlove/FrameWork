//using PrimeTween;
//using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//namespace FrameWork.TweenExtension
//{
//    [System.Serializable]
//    public class NGUISpriteColorTweenCommand : TweenCommand
//    {
//        public UISprite[] targets;

//        public bool isRandomEndColor = false;
//        public Color minEndColor = Color.white;
//        public Color maxEndColor = Color.white;

//        public bool isRandomDuration = false;
//        public float minDuration = 0.5f;
//        public float maxDuration = 1f;

//        public bool isCurve = false;
//        public Ease[] eases;
//        public AnimationCurve[] curves;

//        [HideInInspector] public Color[] _startColors;

//        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1f, float startDelay = 0f)
//        {
//            if (targets == null || targets.Length == 0) return;

//            Color finalColor = isRandomEndColor ?
//                Color.Lerp(minEndColor, maxEndColor, Random.value) : minEndColor;

//            float finalDuration = (isRandomDuration ? Random.Range(minDuration, maxDuration) : minDuration) / timeScale;

//            object selectedEaseOrCurve = Ease.Linear;
//            if (isCurve)
//            {
//                if (curves != null && curves.Length > 0) selectedEaseOrCurve = curves[Random.Range(0, curves.Length)];
//            }
//            else
//            {
//                if (eases != null && eases.Length > 0) selectedEaseOrCurve = eases[Random.Range(0, eases.Length)];
//            }

//            for (int i = 0; i < targets.Length; i++)
//            {
//                if (targets[i] == null) continue;

//                Tween tween;
//                if (selectedEaseOrCurve is AnimationCurve animationCurve)
//                {
//                    tween = Tween.Custom(targets[i], targets[i].color, finalColor, finalDuration, (t, val) => t.color = val, animationCurve, startDelay: startDelay);
//                }
//                else
//                {
//                    tween = Tween.Custom(targets[i], targets[i].color, finalColor, finalDuration, (t, val) => t.color = val, (Ease)selectedEaseOrCurve, startDelay: startDelay);
//                }

//                if (isJoinMode || i > 0) sequence.Group(tween);
//                else sequence.Chain(tween);
//            }
//        }

//        public override void RecordStartValues(bool isForce)
//        {
//            if (targets == null) return;

//            if (_startColors == null || isForce)
//            {
//                _startColors = new Color[targets.Length];
//                for (int i = 0; i < targets.Length; i++)
//                {
//                    if (targets[i] != null) _startColors[i] = targets[i].color;
//                }
//                return;
//            }

//            if (_startColors.Length != targets.Length)
//            {
//                Color[] newStarts = new Color[targets.Length];
//                for (int i = 0; i < targets.Length; i++)
//                {
//                    if (i < _startColors.Length) newStarts[i] = _startColors[i];
//                    else if (targets[i] != null) newStarts[i] = targets[i].color;
//                }
//                _startColors = newStarts;
//            }
//        }

//        public override void RestoreStartValues()
//        {
//            if (targets == null || _startColors == null) return;
//            for (int i = 0; i < targets.Length; i++)
//            {
//                if (targets[i] != null && i < _startColors.Length)
//                    targets[i].color = _startColors[i];
//            }
//        }

//#if UNITY_EDITOR
//        public override string GetDescription() => "NGUI 색상 (Color)";

//        public override void Draw(Rect rect)
//        {
//            float h = EditorGUIUtility.singleLineHeight;
//            float currentY = rect.y;

//            GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel);
//            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.2f, 0.2f);
//            SerializedProperty prop = currentDrawingProperty;

//            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "타겟 (UISprite) 설정", headerStyle);
//            currentY += h + 2;

//            if (prop != null)
//            {
//                SerializedProperty targetsProp = prop.FindPropertyRelative("targets");
//                if (targetsProp != null)
//                {
//                    float height = EditorGUI.GetPropertyHeight(targetsProp, true);
//                    EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, height), targetsProp, true);
//                    currentY += height + 5;
//                }
//            }

//            DrawSeparator(rect.x, currentY, rect.width);
//            currentY += 6;

//            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "목적지 색상", headerStyle);
//            currentY += h + 2;

//            isRandomEndColor = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 랜덤 색상 사용 여부", isRandomEndColor);
//            currentY += h + 2;

//            if (isRandomEndColor)
//            {
//                minEndColor = EditorGUI.ColorField(new Rect(rect.x, currentY, rect.width, h), new GUIContent("최소 색상 (Min)"), minEndColor, true, true, false);
//                currentY += h + 2;
//                maxEndColor = EditorGUI.ColorField(new Rect(rect.x, currentY, rect.width, h), new GUIContent("최대 색상 (Max)"), maxEndColor, true, true, false);
//                currentY += h + 2;
//            }
//            else
//            {
//                minEndColor = EditorGUI.ColorField(new Rect(rect.x, currentY, rect.width, h), new GUIContent("목적지 색상 (Color)"), minEndColor, true, true, false);
//                currentY += h + 2;
//            }

//            DrawSeparator(rect.x, currentY, rect.width);
//            currentY += 6;

//            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "지속 시간", headerStyle);
//            currentY += h + 2;
//            isRandomDuration = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 랜덤 지속시간 사용 여부", isRandomDuration);
//            currentY += h + 2;
//            if (isRandomDuration)
//            {
//                minDuration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "최소 시간 (Min)", minDuration); currentY += h + 2;
//                maxDuration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "최대 시간 (Max)", maxDuration); currentY += h + 2;
//            }
//            else
//            {
//                minDuration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "지속 시간 (Duration)", minDuration); currentY += h + 2;
//            }

//            DrawSeparator(rect.x, currentY, rect.width);
//            currentY += 6;

//            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "애니메이션 동작 방식", headerStyle);
//            currentY += h + 2;
//            isCurve = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 애니메이션 커브(Curve) 사용", isCurve);
//            currentY += h + 2;

//            if (prop != null)
//            {
//                SerializedProperty arrayProp = prop.FindPropertyRelative(isCurve ? "curves" : "eases");
//                if (arrayProp != null) EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, EditorGUI.GetPropertyHeight(arrayProp, true)), arrayProp, true);
//            }
//        }

//        public override int GetNumRows()
//        {
//            if (isFolded) return 0;
//            int rows = 2;
//            SerializedProperty prop = currentDrawingProperty;

//            if (prop != null)
//            {
//                SerializedProperty targetsProp = prop.FindPropertyRelative("targets");
//                if (targetsProp != null) rows += Mathf.CeilToInt(EditorGUI.GetPropertyHeight(targetsProp, true) / (EditorGUIUtility.singleLineHeight + 2));
//            }
//            else rows += 1;

//            rows += 1; rows += 1; rows += isRandomEndColor ? 2 : 1;
//            rows += 1; rows += 1; rows += isRandomDuration ? 2 : 1;
//            rows += 1; rows += 1;

//            if (prop != null)
//            {
//                SerializedProperty arrayProp = prop.FindPropertyRelative(isCurve ? "curves" : "eases");
//                if (arrayProp != null) rows += Mathf.CeilToInt(EditorGUI.GetPropertyHeight(arrayProp, true) / (EditorGUIUtility.singleLineHeight + 2));
//            }
//            else rows += 1;

//            return rows;
//        }
//#endif
//    }
//}