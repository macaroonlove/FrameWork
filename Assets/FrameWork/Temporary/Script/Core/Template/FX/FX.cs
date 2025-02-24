using UnityEngine;

namespace Temporary.Core
{
    public abstract class FX : ScriptableObject
    {
        public abstract void Play(Unit target);

        public abstract void Play(Vector3 pos);
    }
}