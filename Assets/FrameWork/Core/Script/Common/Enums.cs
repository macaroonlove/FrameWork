namespace Temporary.Core
{
    /// <summary>
    /// �̵� ���
    /// (����, ����)
    /// </summary>
    public enum EMoveType
    {
        Ground,
        Sky,
    }

    /// <summary>
    /// ���� ���
    /// (�ٰŸ�, ���Ÿ�, ȸ��, ���ݾ���)
    /// </summary>
    public enum EAttackType
    {
        Near,
        Far,
        Heal,
        None,
    }

    /// <summary>
    /// ������ Ÿ��
    /// (����, ����, ����)
    /// </summary>
    public enum EDamageType
    {
        PhysicalDamage,
        MagicDamage,
        TrueDamage,
    }

    /// <summary>
    /// ���� ȸ�� ���
    /// </summary>
    public enum EManaRecoveryType
    {
        None,
        Automatic,
        Attack,
        Hit,
    }

    /// <summary>
    /// Ÿ�� ���� ���
    /// </summary>
    public enum ETarget
    {
        /// <summary>
        /// �ڱ� �ڽ�
        /// </summary>
        Myself,
        /// <summary>
        /// ���� �� Ÿ�� �ϳ�
        /// </summary>
        OneTargetInRange,
        /// <summary>
        /// ���� �� Ÿ�� (��)��ŭ
        /// </summary>
        NumTargetInRange,
        /// <summary>
        /// ���� �� Ÿ�� ���
        /// </summary>
        AllTargetInRange,
        /// <summary>
        /// ��� Ÿ��
        /// </summary>
        AllTarget,
    }

    /// <summary>
    /// ���� ���
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