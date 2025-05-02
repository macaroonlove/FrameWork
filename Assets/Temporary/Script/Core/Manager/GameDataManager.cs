using Cysharp.Threading.Tasks;
using FrameWork;
using System.Collections.Generic;
using System.Linq;
using Temporary.Save;
using UnityEngine;

namespace Temporary.Core
{
    public class GameDataManager : PersistentSingleton<GameDataManager>
    {
        [Header("SaveData")]
        [SerializeField] private ProfileSaveDataTemplate _profileSaveData;
        [SerializeField] private FormationSaveDataTemplate _formationSaveData;

        [Header("Library")]
        [SerializeField] private AgentLibraryTemplate _agentLibrary;
        [SerializeField] private WaveLibraryTemplate _waveLibrary;

        internal ProfileSaveDataTemplate profileSaveData => _profileSaveData;
        internal List<AgentTemplate> agentTemplate => _agentLibrary.templates;
        internal WaveLibraryTemplate waveLibrary => _waveLibrary;

        internal AgentTemplate GetAgentTemplateById(int id)
        {
            return _agentLibrary.templates.Where(x => x.id == id).FirstOrDefault();
        }

        internal async UniTask<FormationSaveDataTemplate> GetFormationSaveData()
        {
            if (_formationSaveData.isLoaded == false)
            {
                await SaveManager.Instance.Load_FormationData();
            }
            return _formationSaveData;
        }

    }
}
