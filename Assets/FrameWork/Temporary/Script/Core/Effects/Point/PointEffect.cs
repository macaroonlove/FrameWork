using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// 특정 지점으로 효과를 적용
    /// </summary>
    public abstract class PointEffect : Effect
    {
        public abstract void Execute(Unit casterUnit, Vector3 targetVector);
    }
}