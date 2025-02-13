using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class SFXStopEpisode : ThemeEpisode
    {
        public override CommandType command => CommandType.SFX_Stop;
    }
}