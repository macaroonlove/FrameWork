using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public abstract class UnitTrigger : ScriptableObject
    {
        public List<Effect> effects;

        public abstract string GetLabel();

#if UNITY_EDITOR
        public virtual void Draw() { }
#endif
    }
}