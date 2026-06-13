using PrimeTween;
using UnityEngine;

namespace FrameWork.TweenExtension
{
    [System.Serializable]
    public class JoinTweenCommand : TweenCommand
    {
        public override void Execute(ref Sequence sequence, bool isJoinMode, float timeScale = 1, float startDelay = 0f) { }

#if UNITY_EDITOR
        public override string GetDescription() => "동시 실행 (Join) - 다음 트윈을 함께 실행합니다.";

        public override void Draw(Rect rect) { }
        public override float GetHeight() => 0;
#endif
    }
}