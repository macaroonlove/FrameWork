using UnityEngine;

namespace FrameWork.TweenExtension
{
    public interface IColorTweenCommand
    {
        bool IsRandomEndColor { get; }
        Color MinEndColor { get; }
        Color MaxEndColor { get; }
    }
}