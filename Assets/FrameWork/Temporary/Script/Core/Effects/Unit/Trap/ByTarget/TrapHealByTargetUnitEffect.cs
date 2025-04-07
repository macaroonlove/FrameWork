using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class TrapHealByTargetUnitEffect : TrapHealUnitEffect, IGetTarget
    {
        [SerializeField] private GetHealableTargetData _getTargetData = new GetHealableTargetData();

        public List<Unit> GetTarget(Unit casterUnit)
        {
            return _getTargetData.GetTarget(casterUnit);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            rect = _getTargetData.Draw(rect);

            base.Draw(rect);
        }

        public override int GetNumRows()
        {
            return _getTargetData.GetNumRows(base.GetNumRows());
        }
#endif
    }
}