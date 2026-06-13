using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrameWork.TweenExtension
{
    [System.Serializable]
    public class UGUIAnimationTweenCommand : TweenCommand
    {
        public Image target;

        [Tooltip("재생할 프레임들을 순서대로 넣어주세요.")]
        public Sprite[] animationFrames;

        public float duration = 1f;
        public Ease ease = Ease.Linear;

        [HideInInspector] public Sprite _startSprite;
        [HideInInspector] public bool _isBaked = false;

        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1f, float startDelay = 0f)
        {
            if (target == null || animationFrames == null || animationFrames.Length == 0) return;

            float finalDuration = duration / timeScale;
            Sprite[] capturedFrames = animationFrames;

            Tween tween = Tween.Custom(target, 0f, 0.999f, finalDuration, delegate (Image t, float val)
            {
                int frameIndex = Mathf.FloorToInt(val * capturedFrames.Length);
                t.sprite = capturedFrames[frameIndex];
            }, ease, startDelay: startDelay);

            if (isJoinMode) sequence.Group(tween);
            else sequence.Chain(tween);
        }

        public override void RecordStartValues(bool isForce)
        {
            if (target == null) return;

            if (isForce || !_isBaked)
            {
                _startSprite = target.sprite;
                _isBaked = true;
            }
        }

        public override void RestoreStartValues()
        {
            if (target != null && _isBaked)
            {
                target.sprite = _startSprite;
            }
        }

#if UNITY_EDITOR
        public override string GetDescription()
        {
            if (target == null)
                return "UGUI 애니메이션: 타겟 없음";

            string frameText = animationFrames != null
                ? $"{animationFrames.Length}프레임"
                : "0프레임";

            return $"UGUI 애니메이션: {target.name} ({frameText}, {duration:0.##}초)";
        }

        public override void Draw(Rect rect)
        {
            float h = EditorGUIUtility.singleLineHeight;
            float currentY = rect.y;

            GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.2f, 0.2f);
            SerializedProperty prop = currentDrawingProperty;

            // 타겟 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "타겟 (UI Image) 설정", headerStyle);
            currentY += h + 2;

            EditorGUI.BeginChangeCheck();
            target = (Image)EditorGUI.ObjectField(new Rect(rect.x, currentY, rect.width, h), "Target", target, typeof(Image), true);
            if (EditorGUI.EndChangeCheck())
            {
                _isBaked = false;
            }
            currentY += h + 5;

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            // 애니메이션 프레임 설정
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "프레임 시퀀스 (Sprite Array)", headerStyle);
            currentY += h + 2;

            if (prop != null)
            {
                SerializedProperty framesProp = prop.FindPropertyRelative("animationFrames");
                if (framesProp != null)
                {
                    float height = EditorGUI.GetPropertyHeight(framesProp, true);
                    EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, height), framesProp, true);
                    currentY += height + 5;
                }
            }

            DrawSeparator(rect.x, currentY, rect.width);
            currentY += 6;

            // 시간 및 곡선
            EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, h), "재생 설정", headerStyle);
            currentY += h + 2;

            duration = EditorGUI.FloatField(new Rect(rect.x, currentY, rect.width, h), "지속 시간 (Duration)", duration);
            currentY += h + 2;

            ease = (Ease)EditorGUI.EnumPopup(new Rect(rect.x, currentY, rect.width, h), "재생 방식 (Ease)", ease);
        }

        public override int GetNumRows()
        {
            if (isFolded) return 0;
            int rows = 2; // 기본 패딩
            SerializedProperty prop = currentDrawingProperty;

            rows += 1; // target 필드

            rows += 1; // 프레임 타이틀

            // Frames 배열 높이
            if (prop != null)
            {
                SerializedProperty framesProp = prop.FindPropertyRelative("animationFrames");
                if (framesProp != null) rows += Mathf.CeilToInt(EditorGUI.GetPropertyHeight(framesProp, true) / (EditorGUIUtility.singleLineHeight + 2));
            }
            else rows += 1;

            rows += 1; // 재생 설정 타이틀
            rows += 2; // duration, ease

            return rows;
        }
#endif
    }
}