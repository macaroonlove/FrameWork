using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Temporary.Core
{
    public class InputSystem : MonoBehaviour, ISubSystem
    {
        private PlayerInput _playerInput;

        private bool _isSkillActive_1;

        public event UnityAction onSkillPerform_1;
        public event UnityAction onSkillCancel_1;

        public event UnityAction onItem_1;
        public event UnityAction onItem_2;
        public event UnityAction onItem_3;

        public void Initialize()
        {
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput != null)
                _playerInput.enabled = true;
        }

        public void Deinitialize()
        {
            if (_playerInput != null)
                _playerInput.enabled = false;
        }

#if ENABLE_INPUT_SYSTEM
        public void OnSkill1(InputValue value)
        {
            if (value.isPressed && !_isSkillActive_1)
            {
                _isSkillActive_1 = true;
                onSkillPerform_1?.Invoke();
            }
            else if (!value.isPressed && _isSkillActive_1)
            {
                _isSkillActive_1 = false;
                onSkillCancel_1?.Invoke();
            }
        }

        public void OnItem1(InputValue value)
        {
            onItem_1?.Invoke();
        }

        public void OnItem2(InputValue value)
        {
            onItem_2?.Invoke();
        }

        public void OnItem3(InputValue value)
        {
            onItem_3?.Invoke();
        }
#endif
    }
}
