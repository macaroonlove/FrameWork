using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public class ProjectileAbnormalStatusEventSkillEffect : EventSkillEffect
    {
        [SerializeField] private GameObject _prefab;

        [SerializeField] private ETarget _target;
        [SerializeField] private EAttackType _attackType;
        [SerializeField] private float _radius;
        [SerializeField] private int _numberOfTarget;
        [SerializeField] private float _duration;
        [SerializeField] private AbnormalStatusTemplate _abnormalStatus;

        public override string GetDescription()
        {
            return "����ü �����̻� ��ų";
        }

        public override List<Unit> GetTarget(Unit casterUnit)
        {
            return casterUnit.GetAbility<FindTargetAbility>().FindAttackableTarget(_target, _radius, _attackType, _numberOfTarget);
        }

        public override void Execute(Unit casterUnit, Unit targetUnit)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            casterUnit.GetAbility<ProjectileAbility>().SpawnProjectile(_prefab, targetUnit, (caster, target) => { SkillImpact(target); });
        }

        public void SkillImpact(Unit targetUnit)
        {
            targetUnit.GetAbility<AbnormalStatusAbility>().ApplyAbnormalStatus(_abnormalStatus, _duration);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "������");
            _prefab = (GameObject)EditorGUI.ObjectField(valueRect, _prefab, typeof(GameObject), false);

            labelRect.y += 40;
            valueRect.y += 40;
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

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "���� ���");
            _attackType = (EAttackType)EditorGUI.EnumPopup(valueRect, _attackType);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "���ӽð�");
            _duration = EditorGUI.FloatField(valueRect, _duration);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "�����̻�");
            _abnormalStatus = (AbnormalStatusTemplate)EditorGUI.ObjectField(valueRect, _abnormalStatus, typeof(AbnormalStatusTemplate), false);
        }

        public override int GetNumRows()
        {
            int rowNum = 6;

            if (_target != ETarget.Myself && _target != ETarget.AllTarget)
            {
                rowNum++;
            }

            if (_target == ETarget.NumTargetInRange)
            {
                rowNum++;
            }

            return rowNum;
        }
#endif
    }
}
