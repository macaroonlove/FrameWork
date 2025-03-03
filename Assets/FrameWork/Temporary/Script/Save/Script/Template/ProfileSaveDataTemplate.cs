using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.Editor;
using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Save
{
    [Serializable]
    public class ProfileSaveData
    {
        /// <summary>
        /// 골드
        /// </summary>
        public ObscuredInt gold;

        /// <summary>
        /// 보유하고 있는 아군 유닛들
        /// </summary>
        public List<Agent> ownedAgents = new List<Agent>();

        /// <summary>
        /// 보유하고 있는 패시브 아이템들의 아이디
        /// </summary>
        public List<int> ownedPassiveItemIds = new List<int>();

        /// <summary>
        /// 보유하고 있는 액티브 아이템들의 아이디
        /// </summary>
        public List<int> ownedActiveItemIds = new List<int>();

        #region 데이터 모델
        [Serializable]
        public class Agent
        {
            public int id;
            public int unitCount;
            public int level;

            public Agent(int id)
            {
                this.id = id;
                this.unitCount = 1;
                this.level = 1;
            }
        }
        #endregion
    }

    [CreateAssetMenu(menuName = "Templates/SaveData/ProfileSaveData", fileName = "ProfileSaveData", order = 0)]
    public class ProfileSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private ProfileSaveData _data;

        [Header("Variable 연동")]
        [SerializeField] private ObscuredIntVariable _goldVariable;

        public bool isLoaded { get; private set; }

        public override void SetDefaultValues()
        {
            _data = new ProfileSaveData();

            // 초기 캐릭터 추가
            AddAgent(0);
            AddAgent(1);

            // 초기 골드 추가
            _data.gold = 100;

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = JsonUtility.FromJson<ProfileSaveData>(json);

            if (_data != null)
            {
                isLoaded = _data.ownedAgents.Count > 0;

                _goldVariable.Value = _data.gold;
            }

            return isLoaded;
        }

        public override string ToJson()
        {
            if (_data == null) return null;

            _data.gold = _goldVariable.Value;

            return JsonUtility.ToJson(_data);
        }

        public override void Clear()
        {
            _data = null;
            isLoaded = false;
        }

        #region 유닛
        /// <summary>
        /// 유닛을 레벨업 하는데 요구하는 개수
        /// </summary>
        private static readonly int[] _agentUpgradeRequirements = { 0, 1, 3, 5, 7, 10, 15, 30, 50, 90, 150 };

        /// <summary>
        /// 유닛 추가
        /// </summary>
        public void AddAgent(int id)
        {
            var modifyUnit = _data.ownedAgents.Find(x => x.id == id);

            // 유닛이 없었다면 유닛 추가
            if (modifyUnit == null)
            {
                _data.ownedAgents.Add(new ProfileSaveData.Agent(id));
            }
            // 유닛이 있었다면 유닛의 개수 추가
            else
            {
                modifyUnit.unitCount++;
            }
        }

        /// <summary>
        /// 유닛 업그레이드
        /// </summary>
        public bool UpgradeAgent(int id)
        {
            var modifyUnit = _data.ownedAgents.Find(x => x.id == id);

            // 유닛이 있다면 && 최대 레벨이 아니라면
            if (modifyUnit != null && modifyUnit.level < _agentUpgradeRequirements.Length - 1)
            {
                int requiredCount = _agentUpgradeRequirements[modifyUnit.level];

                if (modifyUnit.unitCount >= requiredCount)
                {
                    modifyUnit.unitCount -= requiredCount;
                    modifyUnit.level++;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 업그레이드 가능한 유닛인지 판별
        /// </summary>
        public bool GetUpgradeableUnit(int id)
        {
            var modifyUnit = _data.ownedAgents.Find(x => x.id == id);

            return modifyUnit != null
                && modifyUnit.level < _agentUpgradeRequirements.Length - 1
                && modifyUnit.unitCount >= _agentUpgradeRequirements[modifyUnit.level];
        }
        #endregion

        /// <summary>
        /// 패시브 아이템 추가
        /// </summary>
        public void AddPassiveItem(int id)
        {
            if (_data.ownedPassiveItemIds.Contains(id) == false)
            {
                _data.ownedPassiveItemIds.Add(id);
            }
        }

        /// <summary>
        /// 액티브 아이템 추가
        /// </summary>
        public void AddActiveItem(int id)
        {
            if (_data.ownedActiveItemIds.Contains(id) == false)
            {
                _data.ownedActiveItemIds.Add(id);
            }
        }
    }
}