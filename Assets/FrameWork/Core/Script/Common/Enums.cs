namespace Temporary.Core
{
    /// <summary>
    /// 이동 방식
    /// (지상, 공중)
    /// </summary>
    public enum EMoveType
    {
        Ground,
        Sky,
    }

    /// <summary>
    /// 공격 방식
    /// (근거리, 원거리, 회복, 공격안함)
    /// </summary>
    public enum EAttackType
    {
        Near,
        Far,
        Heal,
        None,
    }

    /// <summary>
    /// 데미지 타입
    /// (물리, 마법, 고정)
    /// </summary>
    public enum EDamageType
    {
        PhysicalDamage,
        MagicDamage,
        TrueDamage,
    }

    /// <summary>
    /// 마나 회복 방식
    /// </summary>
    public enum EManaRecoveryType
    {
        None,
        Automatic,
        Attack,
        Hit,
    }

    /// <summary>
    /// 타겟 선정 방식
    /// </summary>
    public enum ETarget
    {
        /// <summary>
        /// 자기 자신
        /// </summary>
        Myself,
        /// <summary>
        /// 범위 내 타겟 하나
        /// </summary>
        OneTargetInRange,
        /// <summary>
        /// 범위 내 타겟 (수)만큼
        /// </summary>
        NumTargetInRange,
        /// <summary>
        /// 범위 내 타겟 모두
        /// </summary>
        AllTargetInRange,
        /// <summary>
        /// 모든 타겟
        /// </summary>
        AllTarget,
    }

    /// <summary>
    /// 적용 방식
    /// </summary>
    public enum EApplyType
    {
        None,
        ATK,
        FinalATK,
        CurrentHP,
        MAXHP,
        Enemy_CurrentHP,
        Enemy_MAXHP,
    }
}