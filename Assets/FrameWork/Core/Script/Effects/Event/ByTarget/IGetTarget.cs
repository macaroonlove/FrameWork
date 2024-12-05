using System.Collections.Generic;

namespace Temporary.Core
{
    public interface IGetTarget
    {
        public List<Unit> GetTarget(Unit casterUnit);
    }
}