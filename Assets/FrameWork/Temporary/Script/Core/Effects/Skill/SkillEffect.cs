namespace Temporary.Core
{
    public abstract class SkillEffect : Effect
    {
        internal abstract void SkillImpact(Unit casterUnit, Unit targetUnit);
    }
}