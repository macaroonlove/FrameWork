using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.VisualNovel
{
    public enum ChapterState
    {
        None,
        Run,
    }

    public class CommandExecutor : Singleton<CommandExecutor>
    {
        private int _currentIndex = 0;
        private Command _currentCommand;
        private Coroutine _currentCommandCorutine;

        internal ChapterState chapterState = ChapterState.None;

        private List<Command> _commandList = new List<Command>();

        private UnityAction _onEndChapter;

        /// <summary>
        /// 챕터 시작
        /// </summary>
        internal void StartChapter(List<Command> commandList, UnityAction onEndChapter = null)
        {
            // 챕터가 진행 중이라면
            if (chapterState == ChapterState.Run) return;

            _commandList = commandList;
            _currentIndex = 0;
            RegistCommand();

            _onEndChapter = onEndChapter;
        }

        /// <summary>
        /// 다음 커맨드 실행
        /// </summary>
        internal void Next()
        {
            if (_currentIndex < 0) return;

            // 현재 커맨드가 끝났다면
            if (_currentCommand.isComplete)
            {
                RegistCommand();
            }
            // 현재 커맨드가 끝나지 않았다면
            else
            {
                if (_currentCommandCorutine != null)
                {
                    StopCoroutine(_currentCommandCorutine);
                    _currentCommandCorutine = null;
                }

                _currentCommand?.ForceExecute();
            }
        }

        private void RegistCommand()
        {
            // 현재 챕터가 끝났다면
            if (_commandList.Count <= _currentIndex)
            {
                _currentIndex = -1;
                ChapterEnd();
                return;
            }

            _currentCommand = _commandList[_currentIndex];
            _currentCommandCorutine = StartCoroutine(_currentCommand.Execute());
            _currentIndex++;
        }

        /// <summary>
        /// 현재 챕터 스킵
        /// </summary>
        internal void Skip()
        {
            if (_currentCommand.isComplete == false)
            {
                if (_currentCommandCorutine != null)
                {
                    StopCoroutine(_currentCommandCorutine);
                    _currentCommandCorutine = null;
                }

                _currentCommand.isComplete = true;
            }

            ChapterEnd();
        }

        private void ChapterEnd()
        {
            _onEndChapter?.Invoke();
            _onEndChapter = null;
        }

        #region Speak
        internal event UnityAction<string, string, UnityAction, bool> speakStart;
        internal event UnityAction speakEnd;

        internal void Speak(string speaker, string content, UnityAction onSucess, bool isForce = false)
        {
            speakStart?.Invoke(speaker, content, onSucess, isForce);
        }

        internal void SpeakEnd()
        {
            speakEnd?.Invoke();
        }
        #endregion

        #region SCG
        internal event UnityAction<int, string, Rect, Vector2> scgShow;
        internal event UnityAction<int> scgHide;
        internal event Action<int, Rect, Vector2, float, int, Ease, bool, Ease> scgMove;

        internal void SCGShow(int id, string theme, Rect position, Vector2 anchor)
        {
            scgShow?.Invoke(id, theme, position, anchor);
        }

        internal void SCGHide(int id)
        {
            scgHide?.Invoke(id);
        }

        internal void SCGMove(int id, Rect position, Vector2 anchor, float duration, int loopCount, Ease ease, bool isReturn, Ease returnEase)
        {
            scgMove?.Invoke(id, position, anchor, duration, loopCount, ease, isReturn, returnEase);
        }
        #endregion

        #region ECG
        internal event UnityAction<string> ecgShow;
        internal event UnityAction ecgHide;

        internal void ECGShow(string theme)
        {
            ecgShow?.Invoke(theme);
        }

        internal void ECGHide()
        {
            ecgHide?.Invoke();
        }
        #endregion
    }
}