using FrameWork.Editor;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Enemy", fileName = "Enemy", order = 0)]
    public class EnemyTemplate : ScriptableObject
    {
        [Header("�⺻ ����")]
        [SerializeField, Label("�ĺ���ȣ")] private int _id;
        [SerializeField, Label("���� �̸�")] private string _displayName;
        [SerializeField, Label("���� ����"), Multiline(5)] private string _description;

        [Header("���ҽ� ����")]
        [SerializeField, Label("��ǥ �̹���")] private Sprite _sprite;
        [SerializeField, Label("������")] private GameObject _prefab;

        [Header("���� ����")]
        [SerializeField, Label("�̵� �ӵ�")] private float _moveSpeed;
        [SerializeField, Label("�̵� ���")] private EMoveType _moveType;

        [Space(10)]
        [SerializeField, Label("���ݷ�")] private int _atk;
        [SerializeField, Label("���� ����")] private float _attackTerm;
        [SerializeField, Label("���� ��Ÿ�")] private float _attackRange;

        [Space(10)]
        [SerializeField, Label("���� ���")] private EAttackType _attackType;
        [SerializeField, Label("������ Ÿ��")] private EDamageType _damageType;

        [Space(10)]
        [SerializeField, Label("ġ��Ÿ Ȯ��")] private float _criticalHitChance;
        [SerializeField, Label("ġ��Ÿ ������")] private float _criticalHitDamage;

        [Space(10)]
        [SerializeField, Label("���� �����")] private int _physicalPenetration;
        [SerializeField, Label("���� �����")] private int _magicPenetration;

        [Space(10)]
        [SerializeField, Label("�ִ� ü��")] private int _maxHP;
        [SerializeField, Label("����")] private int _physicalResistance;
        [SerializeField, Label("�������׷�")] private int _magicResistance;

        [Space(10)]
        [SerializeField, Label("�ִ� ����")] private int _maxMana;
        [SerializeField, Label("���� ����")] private int _startMana;

        [Space(10)]
        [SerializeField, Label("�ʴ� ü�� ȸ����")] private int _hpRecoveryPerSec;
        [SerializeField, Label("�ʴ� ���� ȸ����")] private int _manaRecoveryPerSec;

        [Space(10)]
        [SerializeField, Label("��ųƮ��")] private SkillTreeGraph _skillTreeGraph;

        #region ������Ƽ
        public int id => _id;
        public string displayName => _displayName;
        public string description => _description;

        public Sprite sprite => _sprite;
        public GameObject prefab => _prefab;

        public float MoveSpeed => _moveSpeed;
        public EMoveType MoveType => _moveType;

        public int ATK => _atk;
        public float AttackTerm => _attackTerm;
        public float AttackRange => _attackRange;

        public EAttackType AttackType => _attackType;
        public EDamageType DamageType => _damageType;

        public float CriticalHitChance => _criticalHitChance;
        public float CriticalHitDamage => _criticalHitDamage;

        public int PhysicalPenetration => _physicalPenetration;
        public int MagicPenetration => _magicPenetration;

        public int MaxHP => _maxHP;
        public int PhysicalResistance => _physicalResistance;
        public int MagicResistance => _magicResistance;

        public int MaxMana => _maxMana;
        public int StartMana => _startMana;

        public int HPRecoveryPerSec => _hpRecoveryPerSec;
        public int ManaRecoveryPerSec => _manaRecoveryPerSec;

        public SkillTreeGraph skillTreeGraph => _skillTreeGraph;
        #endregion
    }
}