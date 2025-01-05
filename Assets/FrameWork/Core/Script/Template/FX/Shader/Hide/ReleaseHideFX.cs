using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/Shader/Hide/Release Hide", fileName = "Release Hide", order = 1)]
    public class ReleaseHideFX : ShaderFX
    {
        public override void Play(Unit target)
        {
            var fxAbility = target.GetAbility<FXAbility>();
            fxAbility.SetShaderKeyword("_ENABLEFULLALPHADISSOLVE_ON", false);
        }
    }
}