using System.Collections.Generic;

namespace Temporary.Core
{
    public abstract class SkillEffect : Effect
    {
        public abstract List<Unit> GetTarget(Unit casterUnit);

        public abstract void Execute(Unit casterUnit, Unit targetUnit);
    }
}