using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// 항상 실행되는 능력
    /// </summary>
    public abstract class AlwaysAbility : Ability
    {
        [SerializeField] internal bool useUpdate = false;
    }
}