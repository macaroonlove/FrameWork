using PrimeTween;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrameWork.TweenExtension
{
    [System.Serializable]
    public class DelayTweenCommand : TweenCommand
    {
        public bool isRandomDelay = false;
        public float minDelay = 0.5f;
        public float maxDelay = 1f;

        public override bool IsDelayCommand => true;

        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1, float startDelay = 0f)
        {
            float finalDelay = (isRandomDelay ? Random.Range(minDelay, maxDelay) : minDelay) / timeScale;
            sequence.ChainDelay(finalDelay);
        }

#if UNITY_EDITOR
        public override string GetDescription()
        {
            if (isRandomDelay)
                return $"{minDelay} ~ {maxDelay}초 대기";
            else
                return $"{minDelay}초 대기";
        }

        public override void Draw(Rect rect)
        {
            float h = EditorGUIUtility.singleLineHeight;
            float currentY = rect.y;

            Rect toggleRect = new Rect(rect.x, currentY, rect.width, h);
            isRandomDelay = EditorGUI.ToggleLeft(toggleRect, "랜덤 딜레이 사용 여부", isRandomDelay);

            currentY += h + 2;

            if (isRandomDelay)
            {
                float halfWidth = rect.width / 2f - 5f;
                Rect r1 = new Rect(rect.x, currentY, halfWidth, h);
                Rect r2 = new Rect(rect.x + halfWidth + 10f, currentY, halfWidth, h);

                float originalLabelWidth = EditorGUIUtility.labelWidth;

                EditorGUIUtility.labelWidth = 100f;

                minDelay = EditorGUI.FloatField(r1, "최소 딜레이", minDelay);
                maxDelay = EditorGUI.FloatField(r2, "최대 딜레이", maxDelay);

                EditorGUIUtility.labelWidth = originalLabelWidth;
            }
            else
            {
                Rect r1 = new Rect(rect.x, currentY, rect.width, h);
                minDelay = EditorGUI.FloatField(r1, "딜레이", minDelay);
            }
        }

        public override int GetNumRows() => 2;
#endif
    }
}