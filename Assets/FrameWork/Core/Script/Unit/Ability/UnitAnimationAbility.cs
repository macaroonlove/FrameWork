using FrameWork;
using UnityEngine;

namespace Temporary.Core
{
    public class UnitAnimationAbility : AlwaysAbility
    {
        private Animator _animator;

        private int _attack;
        private int _moveState;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();

            _attack = Animator.StringToHash("Attack");
            _moveState = Animator.StringToHash("MoveState");
        }

        internal void Attack()
        {
            _animator.SetTrigger(_attack);
        }

        internal void Move(float speed)
        {
            _animator.SetFloat(_moveState, speed);
        }

        internal bool TrySetTrigger(int hash)
        {
            return _animator.TrySetTrigger(hash);
        }
    }
}