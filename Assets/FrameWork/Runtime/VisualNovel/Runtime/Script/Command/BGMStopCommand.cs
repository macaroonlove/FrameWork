using FrameWork.Sound;
using System.Collections;

namespace FrameWork.VisualNovel
{
    public class BGMStopCommand : Command
    {
        internal override IEnumerator Execute()
        {
            SoundManager.StopAllMusic();
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            SoundManager.StopAllMusic();
            isComplete = true;
        }
    }
}