using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public class TrapAbnormalStatusPointEffect : TrapPointEffect
    {
        [SerializeField] private bool _isInfinity;
        [SerializeField] private float _duration;
        [SerializeField] private AbnormalStatusTemplate _abnormalStatus;

        public override string GetDescription()
        {
            return "덫 상태이상 (논타겟팅)";
        }

        protected override void SkillImpact(Unit casterUnit, Unit targetUnit)
        {
            if (_isInfinity)
            {
                targetUnit.GetAbility<AbnormalStatusAbility>().ApplyAbnormalStatus(_abnormalStatus, int.MaxValue);
            }
            else
            {
                targetUnit.GetAbility<AbnormalStatusAbility>().ApplyAbnormalStatus(_abnormalStatus, _duration);
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            var labelRect = new Rect(rect.x, lastRectY, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, lastRectY, rect.width - 140, rect.height);

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "무한지속 사용 여부");
            _isInfinity = EditorGUI.Toggle(valueRect, _isInfinity);
            if (!_isInfinity)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "지속시간");
                _duration = EditorGUI.FloatField(valueRect, _duration);
            }

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "상태이상");
            _abnormalStatus = (AbnormalStatusTemplate)EditorGUI.ObjectField(valueRect, _abnormalStatus, typeof(AbnormalStatusTemplate), false);            
        }

        public override int GetNumRows()
        {
            int rowNum = base.GetNumRows();

            rowNum += 3;

            if (!_isInfinity)
            {
                rowNum++;
            }

            return rowNum;
        }
#endif
    }
}