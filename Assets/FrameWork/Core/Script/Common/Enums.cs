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

}