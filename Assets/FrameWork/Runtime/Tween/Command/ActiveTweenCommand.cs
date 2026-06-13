using PrimeTween;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrameWork.TweenExtension
{
    [System.Serializable]
    public class ActiveTweenCommand : TweenCommand
    {
        public GameObject[] targets;
        public bool targetActiveState = true;

        [HideInInspector] public bool[] _startActiveStates;

        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1f, float startDelay = 0f)
        {
            if (targets == null || targets.Length == 0) return;

            Tween tween = Tween.Delay(startDelay, delegate ()
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null) targets[i].SetActive(targetActiveState);
                }
            });

            if (isJoinMode) sequence.Group(tween);
            else sequence.Chain(tween);
        }

        public override void RecordStartValues(bool isForce)
        {
            if (targets == null) return;

            if (_startActiveStates == null || isForce)
            {
                _startActiveStates = new bool[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null) _startActiveStates[i] = targets[i].activeSelf;
                }
                return;
            }

            if (_startActiveStates.Length != targets.Length)
            {
                bool[] newStarts = new bool[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (i < _startActiveStates.Length) newStarts[i] = _startActiveStates[i];
                    else if (targets[i] != null) newStarts[i] = targets[i].activeSelf;
                }
                _startActiveStates = newStarts;
            }
        }

        public override void RestoreStartValues()
        {
            if (targets == null || _startActiveStates == null) return;
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null && i < _startActiveStates.Length)
                {
                    targets[i].SetActive(_startActiveStates[i]);
                }
            }
        }

#if UNITY_EDITOR
        public override string GetDescription()
        {
            if (targets == null || targets.Length == 0)
                return "활성화 상태: 타겟 없음";

            string targetText;

            if (targets.Length == 1)
                targetText = targets[0] != null ? targets[0].name : "Missing";
            else
                targetText = $"{targets[0]?.name} 외 {targets.Length - 1}개";

            return $"활성화 상태: {targetText} → {(targetActiveState ? "활성화" : "비활성화")}";
        }

        public override void Draw(Rect rect)
        {
            float h = EditorGUIUtility.singleLineHeight;
            float currentY = rect.y;

            GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.2f, 0.2f);
            SerializedProperty prop = currentDrawingProperty;

            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "타겟 (GameObject) 설정", headerStyle);
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

            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "설정할 상태", headerStyle);
            currentY += h + 2;

            targetActiveState = EditorGUI.ToggleLeft(new Rect(rect.x, currentY, rect.width, h), " 오브젝트 켜기 (Active True)", targetActiveState);
        }

        public override int GetNumRows()
        {
            if (isFolded) return 0;
            int rows = 2; // 기본 패딩
            SerializedProperty prop = currentDrawingProperty;

            if (prop != null)
            {
                SerializedProperty targetsProp = prop.FindPropertyRelative("targets");
                if (targetsProp != null) rows += Mathf.CeilToInt(EditorGUI.GetPropertyHeight(targetsProp, true) / (EditorGUIUtility.singleLineHeight + 2));
            }
            else rows += 1;

            rows += 1; // 타이틀
            rows += 1; // 토글

            return rows;
        }
#endif
    }
}