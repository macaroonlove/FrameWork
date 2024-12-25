using FrameWork.Editor;
using FrameWork.Sound;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Template/FX/SFX", fileName = "SFX_", order = 1)]
    public class SFX : FX
    {
        [SerializeField, Label("오디오 소스")] private AudioClip _clip;

        public override void Play(Unit target)
        {
            SoundManager.PlaySound(_clip);
        }

        public override void Play(Vector3 pos)
        {
            SoundManager.PlaySound(_clip);
        }
    }
}