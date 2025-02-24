using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Temporary.Core
{
    public abstract class InstantUnitEffect : UnitEffect
    {
        [SerializeField] protected FX _targetFX;

        public override void Execute(Unit casterUnit, Unit targetUnit)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            SkillImpact(casterUnit, targetUnit);

            ExecuteTargetFX(targetUnit);
        }

        protected abstract void SkillImpact(Unit casterUnit, Unit targetUnit);

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
        protected float lastRectY { get; private set; }

        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "´ë»óÀÚ FX");
            _targetFX = (FX)EditorGUI.ObjectField(valueRect, _targetFX, typeof(FX), false);

            lastRectY = labelRect.y;
        }

        public override int GetNumRows()
        {
            return 1;
        }
#endif
    }
}
