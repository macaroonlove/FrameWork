using PrimeTween;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrameWork.TweenExtension
{
    [System.Serializable]
    public class RotationTweenCommand : TweenCommand
    {
        public Transform[] targets;

        public bool isRandomEndAngles = false;
        public Vector3 minEndAngles;
        public Vector3 maxEndAngles;

        public bool isRandomDuration = false;
        public float minDuration = 0.5f;
        public float maxDuration = 1f;

        public bool isCurve = false;
        public Ease[] eases;
        public AnimationCurve[] curves;

        [HideInInspector] public Vector3[] _startRotations;

        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1, float startDelay = 0f)
        {
            if (targets == null || targets.Length == 0) return;

            Vector3 finalAngles = isRandomEndAngles ?
                new Vector3(Random.Range(minEndAngles.x, maxEndAngles.x), Random.Range(minEndAngles.y, maxEndAngles.y), Random.Range(minEndAngles.z, maxEndAngles.z)) :
                minEndAngles;

            float finalDuration = (isRandomDuration ? Random.Range(minDuration, maxDuration) : minDuration) / timeScale;

            object selectedEaseOrCurve = Ease.Linear;

            if (isCurve)
            {
                if (curves != null && curves.Length > 0)
                {
                    selectedEaseOrCurve = curves[Random.Range(0, curves.Length)];
                }
            }
            else
            {
                if (eases != null && eases.Length > 0)
                {
                    selectedEaseOrCurve = eases[Random.Range(0, eases.Length)];
                }
            }

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] == null) continue;

                Tween tween;
                if (selectedEaseOrCurve is AnimationCurve animationCurve)
                {
                    tween = Tween.LocalRotation(targets[i], Quaternion.Euler(finalAngles), finalDuration, animationCurve, startDelay: startDelay);
                }
                else
                {
                    tween = Tween.LocalRotation(targets[i], Quaternion.Euler(finalAngles), finalDuration, (Ease)selectedEaseOrCurve, startDelay: startDelay);
                }

                if (isJoinMode || i > 0)
                {
                    sequence.Group(tween);
                }
                else
                {
                    sequence.Chain(tween);
                }
            }
        }

        public override void RecordStartValues(bool isForce)
        {
            if (targets == null) return;

            // 초기 배열 생성 (처음 베이크할 때)
            if (_startRotations == null)
            {
                _startRotations = new Vector3[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null) _startRotations[i] = targets[i].localEulerAngles;
                }
                return;
            }

            // 강제 갱신(isForce)이면 전체 덮어쓰기
            if (isForce)
            {
                _startRotations = new Vector3[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null) _startRotations[i] = targets[i].localEulerAngles;
                }
                return;
            }

            // 기존 데이터 유지 + 새로운 타겟만 추가
            if (_startRotations.Length != targets.Length)
            {
                Vector3[] newStarts = new Vector3[targets.Length];

                for (int i = 0; i < targets.Length; i++)
                {
                    if (i < _startRotations.Length)
                    {
                        newStarts[i] = _startRotations[i];
                    }
                    else if (targets[i] != null)
                    {
                        newStarts[i] = targets[i].localEulerAngles;
                    }
                }
                _startRotations = newStarts;
            }
        }

        public override void RestoreStartValues()
        {
            if (targets == null || _startRotations == null) return;
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null && i < _startRotations.Length)
                    targets[i].localEulerAngles = _startRotations[i];
            }
        }

#if UNITY_EDITOR
        public override string GetDescription()
        {
            if (targets == null) return "회전: 타겟 없음";

            string targetText;

            if (targets.Length == 1)
                targetText = targets[0] != null ? targets[0].name : "Missing";
            else
                targetText = $"{targets[0]?.name} 외 {targets.Length - 1}개";

            string rotationText = isRandomEndAngles
                ? $"랜덤({minEndAngles} ~ {maxEndAngles})"
                : $"{minEndAngles}";

            string durationText = isRandomDuration
                ? $"{minDuration:0.##}~{maxDuration:0.##}초"
                : $"{minDuration:0.##}초";

            return $"회전: {targetText} → {rotationText}({durationText})";
        }

        public override void Draw(Rect rect)
        {
            float h = EditorGUIUtility.singleLineHeight;
            float currentY = rect.y;

            // 헤더 전용 스케일 스타일 정의
            GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.2f, 0.2f);

            SerializedProperty prop = currentDrawingProperty;

            // 타겟 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "타겟 설정", headerStyle);
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
            else
            {
                EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "Targets 배열 로드 실패", EditorStyles.boldLabel);
                currentY += h + 2;
            }

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            // 목적지 회전 각도 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "목적지 회전 각도", headerStyle);
            currentY += h + 2;

            isRandomEndAngles = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 랜덤 회전각도 사용 여부", isRandomEndAngles);
            currentY += h + 2;

            if (isRandomEndAngles)
            {
                minEndAngles = EditorGUI.Vector3Field(new Rect(rect.x, currentY, rect.width, h), "최소 회전각도 (Min)", minEndAngles);
                currentY += h + 18;
                maxEndAngles = EditorGUI.Vector3Field(new Rect(rect.x, currentY, rect.width, h), "최대 회전각도 (Max)", maxEndAngles);
                currentY += h + 18;
            }
            else
            {
                minEndAngles = EditorGUI.Vector3Field(new Rect(rect.x, currentY, rect.width, h), "회전각도 (End Angles)", minEndAngles);
                currentY += h + 18;
            }

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            // 지속 시간 설정
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

            // 애니메이션 동작 방식
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "애니메이션 동작 방식", headerStyle);
            currentY += h + 2;

            isCurve = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 애니메이션 커브(Curve) 사용 여부 (체크 해제 시 Ease 사용)", isCurve);
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
            int rows = 0;

            // 레이아웃 보정용 기본 여백 오프셋 추가
            rows += 2;

            // Targets 배열 높이 반영
            SerializedProperty prop = currentDrawingProperty;

            if (prop != null)
            {
                SerializedProperty targetsProp = prop.FindPropertyRelative("targets");
                if (targetsProp != null)
                {
                    float targetHeight = EditorGUI.GetPropertyHeight(targetsProp, true);
                    rows += Mathf.CeilToInt(targetHeight / (EditorGUIUtility.singleLineHeight + 2));
                }
            }
            else
            {
                rows += 1;
            }

            // 목적지 회전값 높이 반영
            rows += 1; // 타이틀 정보 줄
            rows += 1; // 토글 줄
            rows += isRandomEndAngles ? 4 : 2;

            // 시간 높이 반영
            rows += 1; // 타이틀 정보 줄
            rows += 1; // 토글 줄
            rows += isRandomDuration ? 2 : 1;

            // 이지/커브 배열 높이 반영
            rows += 1; // 타이틀 정보 줄
            rows += 1; // 토글 줄
            if (prop != null)
            {
                SerializedProperty arrayProp = prop.FindPropertyRelative(isCurve ? "curves" : "eases");
                if (arrayProp != null)
                {
                    float arrayHeight = EditorGUI.GetPropertyHeight(arrayProp, true);
                    rows += Mathf.CeilToInt(arrayHeight / (EditorGUIUtility.singleLineHeight + 2));
                }
            }
            else
            {
                rows += 1;
            }

            return rows;
        }
#endif
    }
}