using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Temporary.Core
{
    /// <summary>
    /// 유닛의 액티브 스킬 사용을 제어합니다.
    /// </summary>
    public class ActiveSkillAbility : ConditionAbility
    {
        private UnitAnimationAbility _unitAnimationAbility;
        private ManaAbility _manaAbility;
        private BuffAbility _buffAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;

        private ActiveSkillTemplate _template;
        private bool _isSkillActive;

        private SkillEventHandler _skillEventHandler;
        private bool _isEventSkill;

        #region 스탯 계산
        private bool finalIsSkillAble
        {
            get
            {
                // 이미 스킬을 사용 중이라면
                if (_isSkillActive) return false;

                // 스킬 사용 불가 상태이상에 걸렸다면
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
            // 스킬을 사용 중이라면 true
            return _isSkillActive;
        }

        #region 스킬 발동
        internal bool TryExecuteSkill(ActiveSkillTemplate template)
        {
            // 스킬 사용이 불가능하다면
            if (finalIsSkillAble == false) return false;

            // 마나가 부족하다면
            if (_manaAbility.TryExecuteSkill(template.needMana) == false) return false;

            // 스킬의 목표 타겟이 존재한다면
            foreach (var effect in _template.effects)
            {
                var targets = effect.GetTarget(unit);

                if (targets.Count > 0 && targets[0] != null)
                {
                    return SkillAnimation(template);
                }
            }

            return false;
        }

        private bool SkillAnimation(ActiveSkillTemplate template)
        {
            if (_unitAnimationAbility.TrySetTrigger(template.parameterHash) == false) return false;

            _template = template;
            _isSkillActive = true;

            if (!_isEventSkill)
            {
                ExecuteSkill();
            }

            return true;
        }

        private void OnSkillEvent()
        {
            ExecuteSkill();
        }

        private void ExecuteSkill()
        {
            foreach (var effect in _template.effects)
            {
                var targets = effect.GetTarget(unit);

                foreach (var target in targets)
                {
                    effect.Execute(unit, target);
                }
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