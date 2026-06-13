using PrimeTween;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.TweenExtension
{
    public class TweenExecutor : MonoBehaviour
    {
        [Header("재생 및 반복 설정")]
        public bool playOnEnable = true;
        public bool restoreOnStop = true;
        public bool isLoop = false;
        public int cycleCount = 1;

        [Min(0.01f)] public float timeScale = 1f;

        [SerializeReference]
        public List<TweenCommand> commands = new List<TweenCommand>();

        private Sequence mainSequence;
        private bool _isScrubbing = false;

        public bool IsScrubbing => _isScrubbing;

        private void OnEnable()
        {
            if (playOnEnable) Play();
        }

        private void OnDisable()
        {
            StopAllTweens();
        }

        public void Play()
        {
            StopAllTweens();

            int cycles = isLoop ? -1 : (cycleCount > 0 ? cycleCount : 1);
            mainSequence = Sequence.Create(cycles: cycles);

            BuildSequence(ref mainSequence);
        }

        public void StopAllTweens()
        {
            if (mainSequence.isAlive) mainSequence.Stop();
            if (restoreOnStop) RestoreStartValues();
        }

        public void CompleteTweens()
        {
            if (mainSequence.isAlive)
            {
                mainSequence.isPaused = false;
                mainSequence.Complete();
            }
            else
            {
                Sequence tempSeq = Sequence.Create(cycles: 1);
                BuildSequence(ref tempSeq);
                tempSeq.Complete();
            }
        }

        private void BuildSequence(ref Sequence seq)
        {
            bool nextIsJoinMode = false;
            float groupDelayCursor = 0f;

            foreach (var command in commands)
            {
                if (command == null) continue;
                if (command is JoinTweenCommand)
                {
                    nextIsJoinMode = true;
                    continue;
                }

                if (command.IsDelayCommand)
                {
                    var delayCmd = command as DelayTweenCommand;
                    float d = 0f;
                    if (delayCmd != null)
                    {
                        d = (delayCmd.isRandomDelay ? Random.Range(delayCmd.minDelay, delayCmd.maxDelay) : delayCmd.minDelay) / timeScale;
                    }

                    // Join + Delay
                    if (nextIsJoinMode)
                    {
                        groupDelayCursor += d;
                    }
                    // Chain + Delay
                    else
                    {
                        seq.ChainDelay(d);
                        groupDelayCursor = 0f;
                    }
                    continue;
                }

                // Chain
                if (!nextIsJoinMode)
                {
                    groupDelayCursor = 0f;
                }

                command.Execute(ref seq, nextIsJoinMode, timeScale, groupDelayCursor);
                nextIsJoinMode = false;
            }
        }

        public void RecordStartValues(bool isForce)
        {
            foreach (var cmd in commands) if (cmd != null) cmd.RecordStartValues(isForce);
        }

        public void RestoreStartValues()
        {
            foreach (var cmd in commands) if (cmd != null) cmd.RestoreStartValues();
        }

        private void OnDestroy()
        {
            if (mainSequence.isAlive) mainSequence.Stop();
        }

#if UNITY_EDITOR
        public void PlayOneCycle(bool pauseForScrub)
        {
            StopAllTweens();
            mainSequence = Sequence.Create(cycles: 1);
            BuildSequence(ref mainSequence);

            mainSequence.isPaused = pauseForScrub;
        }

        public bool IsSequenceAlive() => mainSequence.isAlive;
        public float GetSequenceProgress() => mainSequence.isAlive ? mainSequence.progress : 0f;

        public void SetSequenceProgress(float p)
        {
            if (mainSequence.isAlive) mainSequence.progress = p;
        }

        public void ToggleScrubMode(bool enable)
        {
            _isScrubbing = enable;
            if (_isScrubbing) PlayOneCycle(true);
            else StopAllTweens();
        }
#endif
    }
}