using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public class InstantUnitEffect : UnitEffect
    {
        [SerializeField] protected FX _targetFX;

        public override string GetDescription()
        {
            return "즉시";
        }

        public override void Execute(Unit casterUnit, Unit targetUnit)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            SkillImpact(casterUnit, targetUnit);

            ExecuteTargetFX(targetUnit);
        }

        #region FX
        private void ExecuteTargetFX(Unit target)
        {
            if (_targetFX != null)
            {
                _targetFX.Play(target);
            }
        }
        #endregion

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "대상자 FX");
            _targetFX = (FX)EditorGUI.ObjectField(valueRect, _targetFX, typeof(FX), false);

            rect.y += 20;
            _effectsList?.DoList(rect);
        }
#endif
    }
}
