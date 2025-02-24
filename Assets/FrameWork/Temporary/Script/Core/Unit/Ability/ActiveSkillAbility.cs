using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// 유닛의 액티브 스킬 사용을 제어합니다.
    /// </summary>
    public class ActiveSkillAbility : ConditionAbility
    {
        private UnitAnimationAbility _unitAnimationAbility;
        private ManaAbility _manaAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;

        private ActiveSkillTemplate _template;

        private bool _isSkillActive;
        private Unit _targetUnit;
        private Vector3 _targetVector;

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

            switch (template.skillType)
            {
                case EActiveSkillType.Instant:
                    return TryExecuteInstantSkill(template);
                case EActiveSkillType.Targeting:
                    return TryExecuteTargetingSkill(template);
                case EActiveSkillType.NonTargeting:
                    return TryExecuteNonTargetingSkill(template);
            }

            return false;
        }

        #region 스킬 발동 방식 별 시도 로직
        private bool TryExecuteInstantSkill(ActiveSkillTemplate template)
        {
            foreach (var effect in template.effects)
            {
                if (effect is IGetTarget targetEffect)
                {
                    var targets = targetEffect.GetTarget(unit);

                    if (targets.Count > 0 && targets[0] != null)
                    {
                        return SkillAnimation(template);
                    }
                }
            }

            return false;
        }

        private bool TryExecuteTargetingSkill(ActiveSkillTemplate template)
        {
            LayerMask layerMask;
            switch (template.unitType)
            {
                case EUnitType.All:
                    layerMask = LayerMask.GetMask("Agent", "Enemy");
                    break;
                case EUnitType.Agent:
                    layerMask = LayerMask.GetMask("Agent");
                    break;
                case EUnitType.Enemy:
                    layerMask = LayerMask.GetMask("Enemy");
                    break;
                default:
                    return false;
            }

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                var distance = Vector3.Distance(unit.transform.position, hit.point);

                if (distance <= template.skillRange)
                {
                    _targetUnit = hit.collider.GetComponent<Unit>();
                    return SkillAnimation(template);
                }
            }

            return false;
        }

        private bool TryExecuteNonTargetingSkill(ActiveSkillTemplate template)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            {
                _targetVector = hit.point;

                return SkillAnimation(template);
            }

            return false;
        }
        #endregion

        private bool SkillAnimation(ActiveSkillTemplate template)
        {
            _template = template;

            bool isSuccess = false;
            if (template.parameterHash != 0 && _isEventSkill)
            {
                isSuccess = _unitAnimationAbility.TrySetTrigger(template.parameterHash);
                _isSkillActive = true;
            }
            
            if (isSuccess == false)
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
            ExecuteCasterFX(_template);

            switch (_template.skillType)
            {
                case EActiveSkillType.Instant:
                    ExecuteInstantSkill(_template);
                    break;
                case EActiveSkillType.Targeting:
                    ExecuteTargetingSkill(_template);
                    break;
                case EActiveSkillType.NonTargeting:
                    ExecuteNonTargetingSkill(_template);
                    break;
            }

            if (!_isEventSkill)
            {
                OnSkillEndEvent();
            }
        }

        #region 스킬 발동 방식 별 실행 로직
        private void ExecuteInstantSkill(ActiveSkillTemplate template)
        {
            foreach (var effect in template.effects)
            {
                if (effect is UnitEffect unitEffect)
                {
                    if (unitEffect is IGetTarget targetEffect)
                    {
                        var targets = targetEffect.GetTarget(unit);

                        foreach (var target in targets)
                        {
                            unitEffect.Execute(unit, target);
                        }
                    }
                }
            }
        }

        private void ExecuteTargetingSkill(ActiveSkillTemplate template)
        {
            foreach (var effect in template.effects)
            {
                if (effect is UnitEffect unitEffect)
                {
                    unitEffect.Execute(unit, _targetUnit);
                }
            }

            ExecuteTargetFX(template, _targetUnit);
        }

        private void ExecuteNonTargetingSkill(ActiveSkillTemplate template)
        {
            foreach (var effect in template.effects)
            {
                if (effect is PointEffect pointEffect)
                {
                    pointEffect.Execute(unit, _targetVector);
                }
            }

            ExecuteTargetFX(template, _targetVector);
        }
        #endregion

        private void OnSkillEndEvent()
        {
            _isSkillActive = false;
        }
        #endregion

        #region FX
        private void ExecuteCasterFX(ActiveSkillTemplate template)
        {
            if (template.casterFX != null)
            {
                template.casterFX.Play(unit);
            }
        }

        private void ExecuteTargetFX(ActiveSkillTemplate template, Unit target)
        {
            if (template.targetFX != null)
            {
                template.targetFX.Play(target);
            }
        }

        private void ExecuteTargetFX(ActiveSkillTemplate template, Vector3 targetVector)
        {
            if (template.targetFX != null)
            {
                template.targetFX.Play(targetVector);
            }
        }
        #endregion
    }
}