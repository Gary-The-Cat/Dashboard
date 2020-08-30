using Shared.Commands.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Commands
{
    public class CommandManager
    {
        private Stack<IUndoableCommand> commands;

        private Stack<IUndoableCommand> undoneCommands;

        public CommandManager()
        {
            commands = new Stack<IUndoableCommand>();
            undoneCommands = new Stack<IUndoableCommand>();
        }

        public bool ExecuteCommand(IUndoableCommand command)
        {
            // Any execution removes all previous undoCommands for redo functionality.
            if (undoneCommands.Any())
            {
                undoneCommands.Clear();
            }

            commands.Push(command);

            return command.Execute();
        }

        public bool? Undo()
        {
            // Ensure that there is a command that can be undone
            if (commands.Any())
            {
                // Remove the command from the commands list
                var command = commands.Pop();

                // Perform the undo
                var successful = command.Undo();
                
                if (successful)
                {
                    // Push the command to our undoCommands list
                    undoneCommands.Push(command);
                }

                // Return if the operation was successful
                return successful;
            }

            // There was no command to undo.
            return null;
        }

        public bool? Redo()
        {
            // If we have any commands
            if (undoneCommands.Any())
            {
                // Grab the command to be redone off of the undoCommands stack
                var command = undoneCommands.Pop();

                // Perform the redo
                var successful = command.Redo(); 

                if (successful)
                {
                    // Push the command to our commands list
                    commands.Push(command);
                }

                // Return if the operation was successful
                return successful;
            }

            // There was no command to undo.
            return null;
        }
    }
}
