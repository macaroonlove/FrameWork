using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// �ð��� ����ϴ� Ŭ����
    /// </summary>
    public class TimeSystem : MonoBehaviour, ISubSystem
    {
        private float _startTime;

        internal float currentTime => Time.time - _startTime;

        public void Initialize()
        {
            _startTime = Time.time;
        }

        public void Deinitialize()
        {

        }
    }
}