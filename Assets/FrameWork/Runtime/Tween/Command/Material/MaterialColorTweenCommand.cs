using PrimeTween;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrameWork.TweenExtension
{
    [System.Serializable]
    public class MaterialColorTweenCommand : TweenCommand, IColorTweenCommand
    {
        public Renderer[] targets;
        public string materialPropertyName = "_BaseColor";

        public bool isRandomEndColor = false;
        [ColorUsage(true, true)] public Color minEndColor = Color.white;
        [ColorUsage(true, true)] public Color maxEndColor = Color.white;

        public bool isRandomDuration = false;
        public float minDuration = 0.5f;
        public float maxDuration = 1f;

        public bool isCurve = false;
        public Ease[] eases;
        public AnimationCurve[] curves;

        [HideInInspector] public Color[] _startColors;

        public bool IsRandomEndColor => isRandomEndColor;
        public Color MinEndColor => minEndColor;
        public Color MaxEndColor => maxEndColor;

        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1f, float startDelay = 0f)
        {
            if (targets == null || targets.Length == 0) return;

            Color finalColor = isRandomEndColor ?
                Color.Lerp(minEndColor, maxEndColor, Random.value) : minEndColor;

            float finalDuration = (isRandomDuration ? Random.Range(minDuration, maxDuration) : minDuration) / timeScale;
            int propId = Shader.PropertyToID(materialPropertyName);

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
                    tween = Tween.MaterialColor(targets[i].material, propId, finalColor, finalDuration, animationCurve, startDelay: startDelay);
                }
                else
                {
                    tween = Tween.MaterialColor(targets[i].material, propId, finalColor, finalDuration, (Ease)selectedEaseOrCurve, startDelay: startDelay);
                }

                if (isJoinMode || i > 0) sequence.Group(tween);
                else sequence.Chain(tween);
            }
        }

        public override void RecordStartValues(bool isForce)
        {
            if (targets == null) return;
            int propId = Shader.PropertyToID(materialPropertyName);

            if (_startColors == null)
            {
                _startColors = new Color[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null && targets[i].sharedMaterial != null)
                        _startColors[i] = targets[i].sharedMaterial.GetColor(propId);
                }
                return;
            }

            if (isForce)
            {
                _startColors = new Color[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null && targets[i].sharedMaterial != null)
                        _startColors[i] = targets[i].sharedMaterial.GetColor(propId);
                }
                return;
            }

            if (_startColors.Length != targets.Length)
            {
                Color[] newStarts = new Color[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (i < _startColors.Length)
                    {
                        newStarts[i] = _startColors[i];
                    }
                    else if (targets[i] != null && targets[i].sharedMaterial != null)
                    {
                        newStarts[i] = targets[i].sharedMaterial.GetColor(propId);
                    }
                }
                _startColors = newStarts;
            }
        }

        public override void RestoreStartValues()
        {
            if (targets == null || _startColors == null) return;
            int propId = Shader.PropertyToID(materialPropertyName);

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null && targets[i].sharedMaterial != null && i < _startColors.Length)
                {
                    targets[i].sharedMaterial.SetColor(propId, _startColors[i]);
                }
            }
        }

#if UNITY_EDITOR
        public override string GetDescription()
        {
            if (targets == null || targets.Length == 0)
                return "Material 색상: 타겟 없음";

            string targetText;

            if (targets.Length == 1)
                targetText = targets[0] != null ? targets[0].name : "Missing";
            else
                targetText = $"{targets[0]?.name} 외 {targets.Length - 1}개";

            string colorText = isRandomEndColor
                ? "랜덤 색상"
                : "색상";

            string durationText = isRandomDuration
                ? $"{minDuration:0.##}~{maxDuration:0.##}초"
                : $"{minDuration:0.##}초";

            return $"Material 투명도: {targetText} → {colorText} ({durationText})";
        }

        public override void Draw(Rect rect)
        {
            float h = EditorGUIUtility.singleLineHeight;
            float currentY = rect.y;

            GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.2f, 0.2f);
            SerializedProperty prop = currentDrawingProperty;

            // 타겟 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "타겟 (Renderer) 및 프로퍼티 설정", headerStyle);
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

            // 쉐이더 프로퍼티 이름 직접 입력란 추가
            materialPropertyName = EditorGUI.TextField(new Rect(rect.x, currentY, rect.width, h), "Property Name", materialPropertyName);
            currentY += h + 2;

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            // 목적지 색상 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "목적지 색상", headerStyle);
            currentY += h + 2;

            isRandomEndColor = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 랜덤 색상 사용 여부", isRandomEndColor);
            currentY += h + 2;

            if (isRandomEndColor)
            {
                minEndColor = EditorGUI.ColorField(new Rect(rect.x, currentY, rect.width, h), new GUIContent("최소 색상 (Min)"), minEndColor, true, true, true);
                currentY += h + 2;
                maxEndColor = EditorGUI.ColorField(new Rect(rect.x, currentY, rect.width, h), new GUIContent("최대 색상 (Max)"), maxEndColor, true, true, true);
                currentY += h + 2;
            }
            else
            {
                minEndColor = EditorGUI.ColorField(new Rect(rect.x, currentY, rect.width, h), new GUIContent("목적지 색상 (Color)"), minEndColor, true, true, true);
                currentY += h + 2;
            }

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            // 시간 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "지속 시간", headerStyle);
            currentY += h + 2;

            isRandomDuration = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 랜덤 지속시간 사용 여부", isRandomDuration);
            currentY += h + 2;

            if (isRandomDuration)
            {
                minDuration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "최소 시간 (Min)", minDuration);
                currentY += h + 2;
                maxDuration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "최대 시간 (Max)", maxDuration);
                currentY += h + 2;
            }
            else
            {
                minDuration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "지속 시간 (Duration)", minDuration);
                currentY += h + 2;
            }

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            // 이즈/커브 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "애니메이션 동작 방식", headerStyle);
            currentY += h + 2;
            isCurve = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 애니메이션 커브(Curve) 사용 여부", isCurve);
            currentY += h + 2;

            if (prop != null)
            {
                if (isCurve)
                {
                    SerializedProperty curvesProp = prop.FindPropertyRelative("curves");
                    if (curvesProp != null)
                    {
                        float height = EditorGUI.GetPropertyHeight(curvesProp, true);
                        EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, height), curvesProp, true);
                    }
                }
                else
                {
                    SerializedProperty easesProp = prop.FindPropertyRelative("eases");
                    if (easesProp != null)
                    {
                        float height = EditorGUI.GetPropertyHeight(easesProp, true);
                        EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, height), easesProp, true);
                    }
                }
            }
        }

        public override int GetNumRows()
        {
            if (isFolded) return 0;
            int rows = 2; // 기본 패딩
            SerializedProperty prop = currentDrawingProperty;

            // 타겟 리스트
            if (prop != null)
            {
                SerializedProperty targetsProp = prop.FindPropertyRelative("targets");
                if (targetsProp != null)
                {
                    float targetHeight = EditorGUI.GetPropertyHeight(targetsProp, true);
                    rows += Mathf.CeilToInt(targetHeight / (EditorGUIUtility.singleLineHeight + 2));
                }
            }
            else rows += 1;

            rows += 1; // materialPropertyName 텍스트 필드

            // 색상 섹션 (타이틀 + 토글 + 컬러필드 1 or 2개)
            rows += 1; rows += 1; rows += isRandomEndColor ? 2 : 1;

            // 시간 섹션 (타이틀 + 토글 + 플로트필드 1 or 2개)
            rows += 1; rows += 1; rows += isRandomDuration ? 2 : 1;

            // 커브 섹션
            rows += 1; rows += 1;
            if (prop != null)
            {
                SerializedProperty arrayProp = prop.FindPropertyRelative(isCurve ? "curves" : "eases");
                if (arrayProp != null)
                {
                    float arrayHeight = EditorGUI.GetPropertyHeight(arrayProp, true);
                    rows += Mathf.CeilToInt(arrayHeight / (EditorGUIUtility.singleLineHeight + 2));
                }
            }
            else rows += 1;

            return rows;
        }
#endif
    }
}