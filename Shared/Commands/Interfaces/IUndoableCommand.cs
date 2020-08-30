using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Commands.Interfaces
{
    public interface IUndoableCommand : ICanExecute, ICanUndo
    {
    }
}
