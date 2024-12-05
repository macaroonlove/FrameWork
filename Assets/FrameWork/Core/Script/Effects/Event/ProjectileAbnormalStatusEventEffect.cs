using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public class ProjectileAbnormalStatusEventEffect : EventEffect
    {
        [SerializeField] protected GameObject _prefab;

        [SerializeField] protected float _duration;
        [SerializeField] protected AbnormalStatusTemplate _abnormalStatus;

        public override string GetDescription()
        {
            return "투사체 상태이상";
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

            GUI.Label(labelRect, "지속시간");
            _duration = EditorGUI.FloatField(valueRect, _duration);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "상태이상");
            _abnormalStatus = (AbnormalStatusTemplate)EditorGUI.ObjectField(valueRect, _abnormalStatus, typeof(AbnormalStatusTemplate), false);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "프리팹");
            _prefab = (GameObject)EditorGUI.ObjectField(valueRect, _prefab, typeof(GameObject), false);
        }

        public override int GetNumRows()
        {
            int rowNum = 2;

            return rowNum;
        }
#endif
    }
}
