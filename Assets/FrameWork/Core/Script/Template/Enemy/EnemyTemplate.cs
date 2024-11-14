using FrameWork.Editor;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Enemy", fileName = "Enemy", order = 0)]
    public class EnemyTemplate : ScriptableObject
    {
        [Header("기본 정보")]
        [SerializeField, Label("식별번호")] private int _id;
        [SerializeField, Label("유닛 이름")] private string _displayName;
        [SerializeField, Label("유닛 설명"), Multiline(5)] private string _description;

        [Header("리소스 정보")]
        [SerializeField, Label("대표 이미지")] private Sprite _sprite;
        [SerializeField, Label("프리팹")] private GameObject _prefab;

        [Header("스탯 정보")]
        [SerializeField, Label("이동 속도")] private float _moveSpeed;
        [SerializeField, Label("이동 방식")] private EMoveType _moveType;

        [Space(10)]
        [SerializeField, Label("공격력")] private int _atk;
        [SerializeField, Label("공격 간격")] private float _attackTerm;
        [SerializeField, Label("공격 사거리")] private float _attackRange;

        [Space(10)]
        [SerializeField, Label("공격 방식")] private EAttackType _attackType;
        [SerializeField, Label("데미지 타입")] private EDamageType _damageType;

        [Space(10)]
        [SerializeField, Label("치명타 확률")] private float _criticalHitChance;
        [SerializeField, Label("치명타 데미지")] private float _criticalHitDamage;

        [Space(10)]
        [SerializeField, Label("물리 관통력")] private int _physicalPenetration;
        [SerializeField, Label("마법 관통력")] private int _magicPenetration;

        [Space(10)]
        [SerializeField, Label("최대 체력")] private int _maxHP;
        [SerializeField, Label("방어력")] private int _physicalResistance;
        [SerializeField, Label("마법저항력")] private int _magicResistance;

        [Space(10)]
        [SerializeField, Label("최대 마나")] private int _maxMana;
        [SerializeField, Label("시작 마나")] private int _startMana;

        [Space(10)]
        [SerializeField, Label("초당 체력 회복량")] private int _hpRecoveryPerSec;
        [SerializeField, Label("초당 마나 회복량")] private int _manaRecoveryPerSec;

        [Space(10)]
        [SerializeField, Label("스킬트리")] private SkillTreeGraph _skillTreeGraph;

        #region 프로퍼티
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