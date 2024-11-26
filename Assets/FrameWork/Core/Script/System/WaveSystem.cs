using System.Collections;
using UnityEngine;

namespace Temporary.Core
{
    public class WaveSystem : MonoBehaviour, ISubSystem
    {
        private EnemySpawnSystem _enemySpawnSystem;
        private TimeSystem _timeSystem;
        private WaveLibraryTemplate _waveLibrary;

        private int _currentWaveIndex;
        private bool _isWaveEnd;

        public void Initialize()
        {
            _enemySpawnSystem = BattleManager.Instance.GetSubSystem<EnemySpawnSystem>();
            _timeSystem = BattleManager.Instance.GetSubSystem<TimeSystem>();
            _waveLibrary = GameDataManager.Instance.waveLibrary;
        }

        public void Deinitialize()
        {
            _enemySpawnSystem = null;
            _timeSystem = null;
        }

        private void Update()
        {
            if (_timeSystem == null) return;
            if (_isWaveEnd) return;

            if (_currentWaveIndex >= _waveLibrary.waves.Count)
            {
                _isWaveEnd = true;
                return;
            }

            if (_timeSystem.currentTime >= _waveLibrary.waves[_currentWaveIndex].spawnTime)
            {
                WaveTemplate currentWave = _waveLibrary.waves[_currentWaveIndex];
                StartCoroutine(SpawnWave(currentWave));
                _currentWaveIndex++;
            }
        }

        private IEnumerator SpawnWave(WaveTemplate wave)
        {
            foreach (var waveInfo in wave.waveInfo)
            {
                // 딜레이 후 적 스폰
                yield return new WaitForSeconds(waveInfo.delayTime);

                var wfs = new WaitForSeconds(waveInfo.spawnInterval);

                for (int i = 0; i < waveInfo.spawnCount; i++)
                {
                    var unit = _enemySpawnSystem.SpawnUnit(waveInfo.template, waveInfo.spawnPosition);

                    // 경로 초기화
                    unit.GetAbility<MoveWayPointAbility>().InitializeWayPoint(waveInfo.wayPoint);

                    if (i < waveInfo.spawnCount - 1)
                    {
                        // 스폰 간격 대기
                        yield return wfs;
                    }
                }
            }
        }
    }
}