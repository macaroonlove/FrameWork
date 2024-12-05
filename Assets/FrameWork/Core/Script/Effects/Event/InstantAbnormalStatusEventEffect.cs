using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public class InstantAbnormalStatusEventEffect : EventEffect
    {
        [SerializeField] protected float _duration;
        [SerializeField] protected AbnormalStatusTemplate _abnormalStatus;

        public override string GetDescription()
        {
            return "즉시 상태이상";
        }

        public override void Execute(Unit casterUnit, Unit targetUnit)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

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
        }

        public override int GetNumRows()
        {
            return 1;
        }
#endif
    }
}
