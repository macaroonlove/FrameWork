using System.Collections.Generic;

namespace Temporary.Core
{
    public abstract class EventEffect : Effect
    {
        public abstract void Execute(Unit casterUnit, Unit targetUnit);
    }
}