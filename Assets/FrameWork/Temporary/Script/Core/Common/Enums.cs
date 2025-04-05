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
        Straight,
        Cone,
        Grid,
    }

    /// <summary>
    /// 액티브 스킬 타입
    /// <para>※ 타겟팅은 적이 있어야만 스킬 발동이 가능합니다.</para>
    /// </summary>
    public enum EActiveSkillType
    {
        /// <summary>
        /// 스킬을 발동하는 유닛의 위치를 기준으로 "유닛"에게 스킬 발동
        /// </summary>
        InstantTargeting,
        /// <summary>
        /// 스킬을 발동하는 유닛의 위치를 기준으로 "위치"에 스킬 발동
        /// </summary>
        InstantNonTargeting,
        /// <summary>
        /// 마우스의 위치를 기준으로 "유닛"에게 스킬 발동
        /// </summary>
        MouseTargeting,
        /// <summary>
        /// 마우스의 위치를 기준으로 "위치"에 스킬 발동
        /// </summary>
        MouseNonTargeting,
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

    /// <summary>
    /// 방향 타입
    /// </summary>
    public enum EDirectionType
    {
        Up,
        Down,
        Left,
        Right,
        Front,
        Back,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        UpFront,
        UpBack,
        DownFront,
        DownBack,
        UpLeftFront,
        UpLeftBack,
        UpRightFront,
        UpRightBack,
        DownLeftFront,
        DownLeftBack,
        DownRightFront,
        DownRightBack,
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