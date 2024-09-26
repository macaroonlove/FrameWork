using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// 시간을 계산하는 클래스
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