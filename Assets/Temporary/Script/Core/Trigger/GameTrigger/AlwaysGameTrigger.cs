using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public class AlwaysGameTrigger : GameTrigger
    {
        public override string GetLabel()
        {
            return "전투 시 상시 적용";
        }
    }
}