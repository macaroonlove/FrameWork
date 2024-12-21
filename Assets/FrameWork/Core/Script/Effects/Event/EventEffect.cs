using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public abstract class EventEffect : Effect
    {
        public abstract void Execute(Unit casterUnit, Unit targetUnit);
    }
}