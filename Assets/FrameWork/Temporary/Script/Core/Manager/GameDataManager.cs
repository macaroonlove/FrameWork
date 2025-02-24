using FrameWork;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Temporary.Core
{
    public class GameDataManager : PersistentSingleton<GameDataManager>
    {
        [SerializeField] private AgentLibraryTemplate _allAgentLibrary;
        [SerializeField] private AgentLibraryTemplate _ownedAgentLibrary;
        [SerializeField] private WaveLibraryTemplate _waveLibrary;

        internal List<AgentTemplate> ownedAgentTemplate => _ownedAgentLibrary.templates;

        internal WaveLibraryTemplate waveLibrary => _waveLibrary;

        internal AgentTemplate GetAllAgentTemplateById(int id)
        {
            return _allAgentLibrary.templates.Where(x => x.id == id).FirstOrDefault();
        }
    }
}
