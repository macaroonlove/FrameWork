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

}