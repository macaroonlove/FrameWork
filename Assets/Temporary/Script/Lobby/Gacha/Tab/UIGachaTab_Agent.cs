using Cysharp.Threading.Tasks;
using FrameWork;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Temporary.Core;
using Temporary.Save;
using UnityEngine;

namespace Temporary.Lobby
{
    /// <summary>
    /// Agent À¯´ÖÀ» »ÌÀ» ¼ö ÀÖ´Â ÅÇ
    /// </summary>
    public class UIGachaTab_Agent : UIGachaTab
    {
        [Header("µî±Þº° È®·üÁ¤º¸")]
        [SerializeField, Range(0, 100.0f)] private int _legendRarity;
        [SerializeField, Range(0, 100.0f)] private int _epicRarity;
        [SerializeField, Range(0, 100.0f)] private int _rareRarity;
        [SerializeField, Range(0, 100.0f)] private int _commonRarity;

        private float[] _rarityProbabilities;
        private List<AgentTemplate> _legendAgents;
        private List<AgentTemplate> _epicAgents;
        private List<AgentTemplate> _rareAgents;
        private List<AgentTemplate> _commonAgents;

        private List<AgentTemplate> _gachaList = new List<AgentTemplate>();

        protected override async void Initialize()
        {
            base.Initialize();

            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);

            _rarityProbabilities = new float[4];
            _rarityProbabilities[0] = _legendRarity;
            _rarityProbabilities[1] = _epicRarity;
            _rarityProbabilities[2] = _rareRarity;
            _rarityProbabilities[3] = _commonRarity;

            var agentList = GameDataManager.Instance.agentTemplates;

            _legendAgents = agentList.Where(template => template.rarity.rarity == ERarity.Legend).ToList();
            _epicAgents = agentList.Where(template => template.rarity.rarity == ERarity.Epic).ToList();
            _rareAgents = agentList.Where(template => template.rarity.rarity == ERarity.Rare).ToList();
            _commonAgents = agentList.Where(template => template.rarity.rarity == ERarity.Common).ToList();
        }

        internal override void PickUp(int gachaCount)
        {
            _gachaList.Clear();
            
            for (int i = 0; i < gachaCount; i++)
            {
                var agent = GetRandomAgent();
                _gachaList.Add(agent);
                GameDataManager.Instance.profileSaveData.AddAgent(agent.id);
            }

            _gachaResultCanvas.Show(_gachaList);

            SaveManager.Instance.Save_ProfileData();
        }

        private AgentTemplate GetRandomAgent()
        {
            int rarityIndex = _rarityProbabilities.Length - 1;

            if (_confirmedPickUpVariable.Value > 0)
            {
                float rand = Random.Range(0, 100.0f);
                float cumulativeProbability = 0;

                _confirmedPickUpVariable.AddValue(-1);

                for (int i = 0; i < _rarityProbabilities.Length; i++)
                {
                    cumulativeProbability += _rarityProbabilities[i];
                    if (rand <= cumulativeProbability)
                    {
                        rarityIndex = i;
                        break;
                    }
                }
            }
            else
            {
                rarityIndex = 0;
            }

            List<AgentTemplate> agents;
            switch (rarityIndex)
            {
                case 0:
                    agents = _legendAgents;
                    _confirmedPickUpVariable.Value = 50;
                    break;
                case 1: agents = _epicAgents; break;
                case 2: agents = _rareAgents; break;
                default: agents = _commonAgents; break;
            }

            var index = Random.Range(0, agents.Count);
            return agents[index];
        }
    }
}