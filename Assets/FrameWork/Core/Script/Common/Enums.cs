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
        Basic,
        ATK,
        FinalATK,
        CurrentHP,
        MAXHP,
        Enemy_CurrentHP,
        Enemy_MAXHP,
    }

    /// <summary>
    /// 적용 방식(타겟 기준으로만 적용)
    /// </summary>
    public enum EApplyType_TargetOnly
    {
        Basic,
        Enemy_CurrentHP,
        Enemy_MAXHP,
    }

    /// <summary>
    /// 등급
    /// </summary>
    public enum ERarity
    {
        Legend,
        Epic,
        Rare,
        Common,
    }

    /// <summary>
    /// 범위 타입
    /// </summary>
    public enum ERangeType
    {
        All,
        Circle,
        //Quad,
    }

    /// <summary>
    /// 액티브 스킬 타입
    /// </summary>
    public enum EActiveSkillType
    {
        /// <summary>
        /// 스킬을 사용하는 유닛의 위치를 기준으로 주위 유닛이 타겟
        /// </summary>
        Instant,
        /// <summary>
        /// 마우스 위치에 유닛이 있다면 해당 유닛이 타겟
        /// </summary>
        Targeting,
        /// <summary>
        /// 마우스 위치를 기준으로 스킬을 발동
        /// <para>Cone형과 Straight형 스킬에 사용됩니다.</para>
        /// </summary>
        NonTargeting,
    }

    /// <summary>
    /// 논타겟 액티브 스킬 타입
    /// </summary>
    public enum ENonTargetingActiveSkillType
    {
        Straight,
        Cone,
    }

    /// <summary>
    /// 유닛 타입
    /// </summary>
    public enum EUnitType
    {
        All,
        Agent,
        Enemy,
        None,
    }

    public enum EOperator
    {
        Add,
        Multiply,
        Set,
    }

    /// <summary>
    /// 유닛을 기준으로 한 투사체, 파티클 등의 스폰 위치
    /// </summary>
    public enum ESpawnPoint
    {
        Head,
        Body,
        RightHand,
        LeftHand,
        Foot,
        ProjectileHit,
    }
}