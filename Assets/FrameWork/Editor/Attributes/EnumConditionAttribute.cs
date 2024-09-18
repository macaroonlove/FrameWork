using System.Collections;
using UnityEngine;

namespace FrameWork.Editor
{
    public class EnumConditionAttribute : PropertyAttribute
    {
        public string ConditionEnum = "";
        public bool Hidden = false;

        BitArray bitArray = new BitArray(32);

        public bool ContainsBitFlag(int enumValue)
        {
            return bitArray.Get(enumValue);
        }

        public EnumConditionAttribute(string conditionBoolean, params int[] enumValues)
        {
            this.ConditionEnum = conditionBoolean;
            this.Hidden = true;

            for (int i = 0; i < enumValues.Length; i++)
            {
                bitArray.Set(enumValues[i], true);
            }
        }
    }

}