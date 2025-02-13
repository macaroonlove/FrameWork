using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    public class VisualNovelManager : MonoBehaviour
    {
        private List<Command> _chapter = new List<Command>();

        public void Load(ChapterTemplate template)
        {
            List<Command> commands = ConvertToCommand(template);

            //_chapter.Add();
        }

        private List<Command> ConvertToCommand(ChapterTemplate template)
        {
            List<Command> commands = new List<Command>();
            //Command command = new CommandStart();

            return commands;
        }
    }
}