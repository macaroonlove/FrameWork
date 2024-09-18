using System;
using UnityEngine;

namespace FrameWork.Editor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionAttribute : PropertyAttribute
    {
        public string ConditionBoolean = "";
        public bool Hidden = false;

        public ConditionAttribute(string conditionBoolean, bool hideInInspector = false)
        {
            this.ConditionBoolean = conditionBoolean;
            this.Hidden = hideInInspector;
        }
    }
}