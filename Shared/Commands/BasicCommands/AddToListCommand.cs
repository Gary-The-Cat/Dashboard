using Shared.Commands.Interfaces;
using System;

namespace Shared.Commands.BasicCommands
{
    public class AddToListCommand<T> : IUndoableCommand
    {
        private T value;
        Action<T> addItem;
        Func<T, bool> removeItem;

        public AddToListCommand(Action<T> addItem, Func<T, bool> removeItem, T value)
        {
            this.addItem = addItem;
            this.removeItem = removeItem;
            this.value = value;
        }

        public bool Execute()
        {
            addItem(value);

            return true;
        }

        public bool Redo()
        {
            addItem(value);

            return true;
        }

        public bool Undo()
        {
            return removeItem(value);
        }
    }
}
