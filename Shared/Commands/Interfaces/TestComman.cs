using Shared.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Commands.Interfaces
{
    public class TestCommand : IUndoableCommand
    {
        public bool Execute()
        {
            throw new NotImplementedException();
        }

        public bool Redo()
        {
            throw new NotImplementedException();
        }

        public bool Undo()
        {
            throw new NotImplementedException();
        }
    }
}
