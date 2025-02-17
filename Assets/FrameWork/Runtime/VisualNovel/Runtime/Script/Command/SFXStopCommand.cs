using FrameWork.Sound;
using System.Collections;

namespace FrameWork.VisualNovel
{
    public class SFXStopCommand : Command
    {
        internal override IEnumerator Execute()
        {
            SoundManager.StopAllSounds();
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            SoundManager.StopAllSounds();
            isComplete = true;
        }
    }
}