using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrameWork.TweenExtension
{
    [System.Serializable]
    public class UGUIAlphaTweenCommand : TweenCommand
    {
        public Graphic[] targets;

        public bool isRandomEndAlpha = false;
        [Range(0f, 1f)] public float minEndAlpha = 0f;
        [Range(0f, 1f)] public float maxEndAlpha = 1f;

        public bool isRandomDuration = false;
        public float minDuration = 0.5f;
        public float maxDuration = 1f;

        public bool isCurve = false;
        public Ease[] eases;
        public AnimationCurve[] curves;

        [HideInInspector] public float[] _startAlphas;

        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1f, float startDelay = 0f)
        {
            if (targets == null || targets.Length == 0) return;

            float finalAlpha = isRandomEndAlpha ? Random.Range(minEndAlpha, maxEndAlpha) : minEndAlpha;
            float finalDuration = (isRandomDuration ? Random.Range(minDuration, maxDuration) : minDuration) / timeScale;

            object selectedEaseOrCurve = Ease.Linear;
            if (isCurve)
            {
                if (curves != null && curves.Length > 0) selectedEaseOrCurve = curves[Random.Range(0, curves.Length)];
            }
            else
            {
                if (eases != null && eases.Length > 0) selectedEaseOrCurve = eases[Random.Range(0, eases.Length)];
            }

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] == null) continue;

                Tween tween;
                if (selectedEaseOrCurve is AnimationCurve animationCurve)
                {
                    tween = Tween.Alpha(targets[i], finalAlpha, finalDuration, animationCurve, startDelay: startDelay);
                }
                else
                {
                    tween = Tween.Alpha(targets[i], finalAlpha, finalDuration, (Ease)selectedEaseOrCurve, startDelay: startDelay);
                }

                if (isJoinMode || i > 0) sequence.Group(tween);
                else sequence.Chain(tween);
            }
        }

        public override void RecordStartValues(bool isForce)
        {
            if (targets == null) return;

            if (_startAlphas == null || isForce)
            {
                _startAlphas = new float[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null) _startAlphas[i] = targets[i].color.a;
                }
                return;
            }

            if (_startAlphas.Length != targets.Length)
            {
                float[] newStarts = new float[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (i < _startAlphas.Length) newStarts[i] = _startAlphas[i];
                    else if (targets[i] != null) newStarts[i] = targets[i].color.a;
                }
                _startAlphas = newStarts;
            }
        }

        public override void RestoreStartValues()
        {
            if (targets == null || _startAlphas == null) return;
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null && i < _startAlphas.Length)
                {
                    Color c = targets[i].color;
                    c.a = _startAlphas[i];
                    targets[i].color = c;
                }
            }
        }

#if UNITY_EDITOR
        public override string GetDescription()
        {
            if (targets == null || targets.Length == 0)
                return "UGUI 투명도: 타겟 없음";

            string targetText;

            if (targets.Length == 1)
                targetText = targets[0] != null ? targets[0].name : "Missing";
            else
                targetText = $"{targets[0]?.name} 외 {targets.Length - 1}개";

            string alphaText = isRandomEndAlpha
                ? $"랜덤({minEndAlpha:0.##} ~ {maxEndAlpha:0.##})"
                : $"{minEndAlpha:0.##}";

            string durationText = isRandomDuration
                ? $"{minDuration:0.##}~{maxDuration:0.##}초"
                : $"{minDuration:0.##}초";

            return $"UGUI 투명도: {targetText} → {alphaText} ({durationText})";
        }

        public override void Draw(Rect rect)
        {
            float h = EditorGUIUtility.singleLineHeight;
            float currentY = rect.y;

            GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.2f, 0.2f);
            SerializedProperty prop = currentDrawingProperty;

            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "타겟 (UI Graphic) 설정", headerStyle);
            currentY += h + 2;

            if (prop != null)
            {
                SerializedProperty targetsProp = prop.FindPropertyRelative("targets");
                if (targetsProp != null)
                {
                    float height = EditorGUI.GetPropertyHeight(targetsProp, true);
                    EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, height), targetsProp, true);
                    currentY += height + 5;
                }
            }

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "목적지 투명도 (0.0 ~ 1.0)", headerStyle);
            currentY += h + 2;

            isRandomEndAlpha = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 랜덤 투명도 사용 여부", isRandomEndAlpha);
            currentY += h + 2;

            if (isRandomEndAlpha)
            {
                minEndAlpha = EditorGUI.Slider(new Rect(rect.x, currentY, rect.width, h), "최소 투명도 (Min)", minEndAlpha, 0f, 1f); currentY += h + 2;
                maxEndAlpha = EditorGUI.Slider(new Rect(rect.x, currentY, rect.width, h), "최대 투명도 (Max)", maxEndAlpha, 0f, 1f); currentY += h + 2;
            }
            else
            {
                minEndAlpha = EditorGUI.Slider(new Rect(rect.x, currentY, rect.width, h), "목적지 투명도 (Alpha)", minEndAlpha, 0f, 1f); currentY += h + 2;
            }

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "지속 시간", headerStyle);
            currentY += h + 2;
            isRandomDuration = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 랜덤 지속시간 사용 여부", isRandomDuration);
            currentY += h + 2;
            if (isRandomDuration)
            {
                minDuration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "최소 시간 (Min)", minDuration); currentY += h + 2;
                maxDuration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "최대 시간 (Max)", maxDuration); currentY += h + 2;
            }
            else
            {
                minDuration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "지속 시간 (Duration)", minDuration); currentY += h + 2;
            }

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "애니메이션 동작 방식", headerStyle);
            currentY += h + 2;
            isCurve = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 애니메이션 커브(Curve) 사용", isCurve);
            currentY += h + 2;

            if (prop != null)
            {
                SerializedProperty arrayProp = prop.FindPropertyRelative(isCurve ? "curves" : "eases");
                if (arrayProp != null) EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, EditorGUI.GetPropertyHeight(arrayProp, true)), arrayProp, true);
            }
        }

        public override int GetNumRows()
        {
            if (isFolded) return 0;
            int rows = 2;
            SerializedProperty prop = currentDrawingProperty;

            if (prop != null)
            {
                SerializedProperty targetsProp = prop.FindPropertyRelative("targets");
                if (targetsProp != null) rows += Mathf.CeilToInt(EditorGUI.GetPropertyHeight(targetsProp, true) / (EditorGUIUtility.singleLineHeight + 2));
            }
            else rows += 1;

            rows += 1; rows += 1; rows += isRandomEndAlpha ? 2 : 1;
            rows += 1; rows += 1; rows += isRandomDuration ? 2 : 1;
            rows += 1; rows += 1;

            if (prop != null)
            {
                SerializedProperty arrayProp = prop.FindPropertyRelative(isCurve ? "curves" : "eases");
                if (arrayProp != null) rows += Mathf.CeilToInt(EditorGUI.GetPropertyHeight(arrayProp, true) / (EditorGUIUtility.singleLineHeight + 2));
            }
            else rows += 1;

            return rows;
        }
#endif
    }
}