using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public class InstantDamageSkillEffect : SkillEffect
    {
        [SerializeField] private ETarget _target;
        [SerializeField] private EAttackType _attackType;
        [SerializeField] private float _radius;
        [SerializeField] private int _numberOfTarget;
        [SerializeField] private int _repeatCount;
        [SerializeField] private bool _isTick;
        [SerializeField] private int _tickCycle;
        [SerializeField] private int _tickCount;
        [SerializeField] private EDamageType _damageType;
        [SerializeField] private EApplyType _applyType;
        [SerializeField] private float _amount;

        public override string GetDescription()
        {
            return "��� ������ ��ų";
        }

        public override List<Unit> GetTarget(Unit casterUnit)
        {
            return casterUnit.GetAbility<FindTargetAbility>().FindAttackableTarget(_target, _radius, _attackType, _numberOfTarget);
        }

        public int GetAmount(Unit casterUnit, Unit targetUnit)
        {
            int amount;
            float typeValue = 0f;
            switch (_applyType)
            {
                case EApplyType.None:
                    return (int)_amount;
                case EApplyType.ATK:
                    typeValue = casterUnit.GetAbility<AttackAbility>().baseATK;
                    break;
                case EApplyType.FinalATK:
                    typeValue = casterUnit.GetAbility<AttackAbility>().finalATK;
                    break;
                case EApplyType.CurrentHP:
                    typeValue = casterUnit.GetAbility<HealthAbility>().currentHP;
                    break;
                case EApplyType.MAXHP:
                    typeValue = casterUnit.GetAbility<HealthAbility>().finalMaxHP;
                    break;
                case EApplyType.Enemy_CurrentHP:
                    typeValue = targetUnit.GetAbility<HealthAbility>().currentHP;
                    break;
                case EApplyType.Enemy_MAXHP:
                    typeValue = targetUnit.GetAbility<HealthAbility>().finalMaxHP;
                    break;
            }

            amount = (int)(typeValue * _amount);

            return amount;
        }

        public override void Execute(Unit casterUnit, Unit targetUnit)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            int damage = GetAmount(casterUnit, targetUnit);

            Execute_RepeatCount(casterUnit, targetUnit, damage);
        }

        private void Execute_RepeatCount(Unit casterUnit, Unit targetUnit, int damage)
        {
            if (_repeatCount > 1)
            {
                for (int i = 0; i < _repeatCount; i++)
                {
                    if (targetUnit.isDie) return;

                    Execute_Tick(casterUnit, targetUnit, damage);
                }
            }
            else
            {
                Execute_Tick(casterUnit, targetUnit, damage);
            }
        }        

        private void Execute_Tick(Unit casterUnit, Unit targetUnit, int damage)
        {
            if (_isTick)
            {
                targetUnit.StartCoroutine(CoExecute_Tick(casterUnit, targetUnit, damage));
            }
            else
            {
                Execute_DamageType(casterUnit, targetUnit, damage);
            }
        }

        private IEnumerator CoExecute_Tick(Unit casterUnit, Unit targetUnit, int damage)
        {
            var wfs = new WaitForSeconds(_tickCycle);

            for (int i = 0; i < _tickCount; i++)
            {
                if (targetUnit.isDie) yield break;

                Execute_DamageType(casterUnit, targetUnit, damage);
                yield return wfs;
            }
        }

        private void Execute_DamageType(Unit casterUnit, Unit targetUnit, int damage)
        {
            if (_damageType == EDamageType.TrueDamage)
            {
                targetUnit.GetAbility<HitAbility>().Hit(damage, casterUnit.id);
            }
            else
            {
                targetUnit.GetAbility<HitAbility>().Hit(damage, _damageType, casterUnit.id);
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "���� ���");
            _attackType = (EAttackType)EditorGUI.EnumPopup(valueRect, _attackType);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "���� ���");
            _target = (ETarget)EditorGUI.EnumPopup(valueRect, _target);

            if (_target != ETarget.Myself && _target != ETarget.AllTarget)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "����");
                _radius = EditorGUI.FloatField(valueRect, _radius);
            }

            if (_target == ETarget.NumTargetInRange)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "������ ���� ��");
                _numberOfTarget = EditorGUI.IntField(valueRect, _numberOfTarget);
            }

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "���� Ƚ��");
            _repeatCount = EditorGUI.IntField(valueRect, _repeatCount);
            if (_repeatCount <= 0) _repeatCount = 1;

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "�ֱ⸶�� ���� ��� ����");
            _isTick = EditorGUI.Toggle(valueRect, _isTick);
            if (_isTick)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "�ֱ�(��)");
                _tickCycle = EditorGUI.IntField(valueRect, _tickCycle);

                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "�ֱ⸶�� ���� Ƚ��");
                _tickCount = EditorGUI.IntField(valueRect, _tickCount);
            }

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "������ Ÿ��");
            _damageType = (EDamageType)EditorGUI.EnumPopup(valueRect, _damageType);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "���� ���");
            _applyType = (EApplyType)EditorGUI.EnumPopup(valueRect, _applyType);

            labelRect.y += 20;
            valueRect.y += 20;
            if (_applyType == EApplyType.None) GUI.Label(labelRect, "���ط�");
            else GUI.Label(labelRect, "���ط�(���)");
            _amount = EditorGUI.FloatField(valueRect, _amount);
        }

        public override int GetNumRows()
        {
            int rowNum = 11;

            if (_target != ETarget.Myself && _target != ETarget.AllTarget)
            {
                rowNum++;
            }

            if (_target == ETarget.NumTargetInRange)
            {
                rowNum++;
            }

            if (_isTick)
            {
                rowNum += 2;
            }

            return rowNum;
        }
#endif
    }
}
