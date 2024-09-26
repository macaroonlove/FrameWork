using UnityEngine;

namespace Temporary.Core
{
    public class UnitAnimationAbility : AlwaysAbility
    {
        private Animator _animator;

        private int _attack;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();

            _attack = Animator.StringToHash("attack");
        }

        internal void Attack()
        {
            _animator.SetTrigger(_attack);
        }
    }
}