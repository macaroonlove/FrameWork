using PrimeTween;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrameWork.TweenExtension
{
    [System.Serializable]
    public class PositionTweenCommand : TweenCommand
    {
        public Transform[] targets;

        public bool isRandomEndPosition = false;
        public Vector3 minEndPosition;
        public Vector3 maxEndPosition;

        public bool isRandomDuration = false;
        public float minDuration = 0.5f;
        public float maxDuration = 1f;

        public bool isCurve = false;
        public Ease[] eases;
        public AnimationCurve[] curves;

        [HideInInspector] public Vector3[] _startPositions;

        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1, float startDelay = 0f)
        {
            if (targets == null || targets.Length == 0) return;

            Vector3 finalPosition = isRandomEndPosition ?
                new Vector3(Random.Range(minEndPosition.x, maxEndPosition.x), Random.Range(minEndPosition.y, maxEndPosition.y), Random.Range(minEndPosition.z, maxEndPosition.z)) :
                minEndPosition;

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
                    tween = Tween.LocalPosition(targets[i], finalPosition, finalDuration, animationCurve, startDelay: startDelay);
                }
                else
                {
                    tween = Tween.LocalPosition(targets[i], finalPosition, finalDuration, (Ease)selectedEaseOrCurve, startDelay: startDelay);
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
            if (_startPositions == null)
            {
                _startPositions = new Vector3[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null) _startPositions[i] = targets[i].localPosition;
                }
                return;
            }

            // 강제 갱신(isForce)이면 전체 덮어쓰기
            if (isForce)
            {
                _startPositions = new Vector3[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null) _startPositions[i] = targets[i].localPosition;
                }
                return;
            }

            // 기존 데이터 유지 + 새로운 타겟만 추가
            if (_startPositions.Length != targets.Length)
            {
                Vector3[] newStarts = new Vector3[targets.Length];

                for (int i = 0; i < targets.Length; i++)
                {
                    // 기존 범위 내에 있고 값이 유효하면 유지
                    if (i < _startPositions.Length)
                    {
                        newStarts[i] = _startPositions[i];
                    }
                    // 새로 추가된 타겟이면 신규 값 베이크
                    else if (targets[i] != null)
                    {
                        newStarts[i] = targets[i].localPosition;
                    }
                }
                _startPositions = newStarts;
            }
        }

        public override void RestoreStartValues()
        {
            if (targets == null || _startPositions == null) return;
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null && i < _startPositions.Length)
                    targets[i].localPosition = _startPositions[i];
            }
        }
#if UNITY_EDITOR
        public override string GetDescription()
        {
            if (targets == null) return "이동: 타겟 없음";

            string targetText;

            if (targets.Length == 1)
                targetText = targets[0] != null ? targets[0].name : "Missing";
            else
                targetText = $"{targets[0]?.name} 외 {targets.Length - 1}개";

            string positionText = isRandomEndPosition
                ? $"랜덤({minEndPosition} ~ {maxEndPosition})"
                : $"{minEndPosition}";

            string durationText = isRandomDuration
                ? $"{minDuration:0.##}~{maxDuration:0.##}초"
                : $"{minDuration:0.##}초";

            return $" 이동: {targetText} → {positionText} ({durationText})";
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

            // 목적지 좌표 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "목적지 위치", headerStyle);
            currentY += h + 2;

            isRandomEndPosition = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 랜덤 목적지 사용 여부", isRandomEndPosition);
            currentY += h + 2;

            if (isRandomEndPosition)
            {
                minEndPosition = EditorGUI.Vector3Field(new Rect(rect.x, currentY, rect.width, h), "최소 목적지 (Min)", minEndPosition);
                currentY += h + 18;
                maxEndPosition = EditorGUI.Vector3Field(new Rect(rect.x, currentY, rect.width, h), "최대 목적지 (Max)", maxEndPosition);
                currentY += h + 18;
            }
            else
            {
                minEndPosition = EditorGUI.Vector3Field(new Rect(rect.x, currentY, rect.width, h), "목적지 (End Position)", minEndPosition);
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

            // 이징 및 커브 설정 (Ease/Curve 배열)
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

            rows += 2;

            // Targets 배열 높이
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

            // 목적지
            rows += 1; // 타이틀 정보
            rows += 1; // 토글
            rows += isRandomEndPosition ? 4 : 2;

            // 시간
            rows += 1; // 타이틀 정보
            rows += 1; // 토글
            rows += isRandomDuration ? 2 : 1;

            // 이지/커브
            rows += 1; // 타이틀 정보
            rows += 1; // 토글
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