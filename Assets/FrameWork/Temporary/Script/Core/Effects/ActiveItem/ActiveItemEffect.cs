using System.Collections.Generic;

namespace Temporary.Core
{
    public abstract class ActiveItemEffect : Effect
    {
        public abstract void Execute(List<Unit> targetUnits);
    }
}