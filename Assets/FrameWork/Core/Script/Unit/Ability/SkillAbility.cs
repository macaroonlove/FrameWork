using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    /// <summary>
    /// ������ ��ų ����� �����մϴ�.
    /// </summary>
    public class SkillAbility : ConditionAbility
    {
        private UnitAnimationAbility _unitAnimationAbility;
        private ManaAbility _manaAbility;
        private BuffAbility _buffAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;

        private SkillTemplate _template;
        private bool _isSkillActive;

        private SkillEventHandler _skillEventHandler;
        private bool _isEventSkill;

        #region ���� ���
        private bool finalIsSkillAble
        {
            get
            {
                // �̹� ��ų�� ��� ���̶��
                if (_isSkillActive) return false;

                // ��ų ��� �Ұ� �����̻� �ɷȴٸ�
                if (_abnormalStatusAbility.UnableToSkillEffects.Count > 0) return false;

                return true;
            }
        }
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _unitAnimationAbility = unit.GetAbility<UnitAnimationAbility>();
            _manaAbility = unit.GetAbility<ManaAbility>();
            _buffAbility = unit.GetAbility<BuffAbility>();
            _abnormalStatusAbility = unit.GetAbility<AbnormalStatusAbility>();

            _skillEventHandler = GetComponentInChildren<SkillEventHandler>();
            if (_skillEventHandler != null)
            {
                _skillEventHandler.onSkill += OnSkillEvent;
                _skillEventHandler.onSkillEnd += OnSkillEndEvent;
                _isEventSkill = true;
            }
        }

        internal override void Deinitialize()
        {
            if (_skillEventHandler != null)
            {
                _skillEventHandler.onSkill -= OnSkillEvent;
                _skillEventHandler.onSkillEnd -= OnSkillEndEvent;
                _isEventSkill = false;
            }
        }

        internal override bool IsExecute()
        {
            // ��ų�� ��� ���̶�� true
            return _isSkillActive;
        }

        #region ��ų �ߵ�
        internal bool TryExecuteSkill(SkillTemplate template)
        {
            // ��ų ����� �Ұ����ϴٸ�
            if (finalIsSkillAble == false) return false;

            // ������ �����ϴٸ�
            if (_manaAbility.TryExecuteSkill(template.needMana) == false) return false;

            _template = template;

            SkillAnimation();            

            return true;
        }

        private void SkillAnimation()
        {
            if (_unitAnimationAbility.TrySetTrigger(_template.parameterHash) == false) return;
            
            _isSkillActive = true;

            if (!_isEventSkill)
            {
                ExecuteSkill();
            }
        }

        private void OnSkillEvent()
        {
            ExecuteSkill();
        }

        private void ExecuteSkill()
        {
            foreach (var effect in _template.effects)
            {
                effect.Execute(unit);
            }

            if (!_isEventSkill)
            {
                OnSkillEndEvent();
            }
        }

        private void OnSkillEndEvent()
        {
            _isSkillActive = false;
        }
        #endregion
    }
}