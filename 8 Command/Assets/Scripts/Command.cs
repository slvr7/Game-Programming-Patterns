using System.Collections.Generic;


namespace GPP
{
    // Not much to the Command class. Just methods for doing and undoing the command
    // all the actual work is defined in subclasses
    public abstract class Command
    {
        public abstract void Do();
        public abstract void Undo();
    }

    public class Commands
    {
        private const uint DefaultUndoLevels = 10;
        private readonly uint _numUndoLevels;
        private readonly List<Command> _history = new List<Command>();

        public Commands(uint numUndoLevels = DefaultUndoLevels)
        {
            _numUndoLevels = numUndoLevels;
        }

        public void Do(Command command)
        {
            command.Do();
            _history.Add(command);
            if (_history.Count > _numUndoLevels)
            {
                _history.RemoveAt(0);
            }
        }

        public void Undo()
        {
            if (_history.Count > 0)
                PopLast().Undo();
        }

        public void Clear()
        {
            _history.Clear();
        }

        private Command PopLast()
        {
            var lastIndex = _history.Count - 1;
            var lastCommand = _history[lastIndex];
            _history.RemoveAt(lastIndex);
            return lastCommand;
        }

    }


}
