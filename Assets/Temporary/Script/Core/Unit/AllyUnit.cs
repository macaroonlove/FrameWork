using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// ¾Æ±º À¯´Ö (Agent, Summon)
    /// </summary>
    public abstract class AllyUnit : Unit
    {
        internal abstract EMoveType moveType { get; }
    }
}