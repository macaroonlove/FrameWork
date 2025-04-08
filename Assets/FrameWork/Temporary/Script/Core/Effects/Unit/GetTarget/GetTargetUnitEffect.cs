using System;
using System.Collections.Generic;
using Temporary.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Temporary.Core
{
    public abstract class GetTargetUnitEffect : UnitEffect
    {
        [SerializeField] protected GetTargetData _getTargetData = new GetTargetData();

        public List<Unit> GetTarget(Unit casterUnit)
        {
            return _getTargetData.GetTarget(casterUnit);
        }
    }
}