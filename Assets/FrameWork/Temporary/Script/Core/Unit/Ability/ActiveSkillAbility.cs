using System.Collections.Generic;
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

        private bool _isExecuteSkill;
        private Unit _targetUnit;
        private Vector3 _targetVector;

        private Dictionary<int, ActiveSkillTemplate> _templates = new Dictionary<int, ActiveSkillTemplate>();

        #region 스탯 계산
        private bool finalIsSkillAble
        {
            get
            {
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
        }

        internal override void Deinitialize()
        {

        }

        internal override bool IsExecute()
        {
            // 스킬을 사용 중이라면 true
            return _isExecuteSkill;
        }

        #region 스킬 발동
        internal bool TryExecuteSkill(ActiveSkillTemplate template)
        {
            // 스킬 사용이 불가능하다면
            if (finalIsSkillAble == false) return false;

            // 마나가 부족하다면
            if (_manaAbility.TryExecuteSkill(template.needMana) == false) return false;

            // 애니메이션이 있는 스킬인데, 이미 스킬을 사용 중이라면
            if (template.parameterHash != 0 && _isExecuteSkill) return false;

            switch (template.skillType)
            {
                case EActiveSkillType.InstantTargeting:
                    return TryExecuteInstantTargetingSkill(template);
                case EActiveSkillType.InstantNonTargeting:
                    return SkillAnimation(template);
                case EActiveSkillType.MouseTargeting:
                    return TryExecuteMouseTargetingSkill(template);
                case EActiveSkillType.MouseNonTargeting:
                    return TryExecuteMouseNonTargetingSkill(template);
            }

            return false;
        }

        #region 스킬 발동 방식별 시도 로직
        private bool TryExecuteInstantTargetingSkill(ActiveSkillTemplate template)
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

        private bool TryExecuteMouseTargetingSkill(ActiveSkillTemplate template)
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

        private bool TryExecuteMouseNonTargetingSkill(ActiveSkillTemplate template)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            {
                _targetVector = hit.point.normalized;

                return SkillAnimation(template);
            }

            return false;
        }
        #endregion

        private bool SkillAnimation(ActiveSkillTemplate template)
        {
            bool isSuccess = false;
            if (template.parameterHash != 0)
            {
                isSuccess = _unitAnimationAbility.TrySetTrigger(template.parameterHash);
            }

            if (isSuccess == true)
            {
                _unitAnimationAbility.SetSkillID(template.id);
                _templates[template.id] = template;
                _isExecuteSkill = true;
            }
            else
            {
                ExecuteSkill(template);
            }

            return true;
        }

        internal void ExecuteSkill(int skillTemplateID)
        {
            ExecuteSkill(_templates[skillTemplateID]);
        }

        private void ExecuteSkill(ActiveSkillTemplate template)
        {
            ExecuteCasterFX(template);

            switch (template.skillType)
            {
                case EActiveSkillType.InstantTargeting:
                    ExecuteInstantTargetingSkill(template);
                    break;
                case EActiveSkillType.InstantNonTargeting:
                    ExecuteInstantNonTargetingSkill(template);
                    break;
                case EActiveSkillType.MouseTargeting:
                    ExecuteMouseTargetingSkill(template);
                    break;
                case EActiveSkillType.MouseNonTargeting:
                    ExecuteMouseNonTargetingSkill(template);
                    break;
            }
        }

        #region 스킬 발동 방식 별 실행 로직
        private void ExecuteInstantTargetingSkill(ActiveSkillTemplate template)
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

        private void ExecuteInstantNonTargetingSkill(ActiveSkillTemplate template)
        {
            foreach (var effect in template.effects)
            {
                if (effect is PointEffect pointEffect)
                {
                    pointEffect.Execute(unit, Vector3.zero);
                }
            }
        }

        private void ExecuteMouseTargetingSkill(ActiveSkillTemplate template)
        {
            foreach (var effect in template.effects)
            {
                if (effect is UnitEffect unitEffect)
                {
                    unitEffect.Execute(unit, _targetUnit);
                }
            }
        }

        private void ExecuteMouseNonTargetingSkill(ActiveSkillTemplate template)
        {
            foreach (var effect in template.effects)
            {
                if (effect is PointEffect pointEffect)
                {
                    pointEffect.Execute(unit, _targetVector);
                }
            }
        }
        #endregion

        internal void EndSkill()
        {
            _isExecuteSkill = false;
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
        #endregion
    }
}