using System.Collections;
using System.Collections.Generic;

namespace FrameWork.VisualNovel
{
    public abstract class Command
    {
        public static LinkedCommand operator +(Command origin, Command append)
        {
            return new LinkedCommand(new List<Command> { origin, append });
        }

        protected bool _isComplete;

        internal abstract IEnumerator Execute();

        internal abstract void ForceExecute();
    }

    public class LinkedCommand : Command
    {
        private List<Command> _commands;

        public LinkedCommand(List<Command> commands)
        {
            _commands = commands;
        }

        internal override IEnumerator Execute()
        {
            foreach (var command in _commands)
            {
                yield return command.Execute();
            }
            _isComplete = true;
        }

        internal override void ForceExecute()
        {
            foreach (var command in _commands)
            {
                command.ForceExecute();
            }
            _isComplete = true;
        }
    }
}