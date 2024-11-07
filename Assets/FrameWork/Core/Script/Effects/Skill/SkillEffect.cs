using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public abstract class SkillEffect : Effect
    {
        public abstract bool Execute(Unit casterUnit);
    }
}
