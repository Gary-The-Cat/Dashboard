namespace Shared.Commands.Interfaces
{
    public interface ICanExecute
    {
        public bool Execute();

        public bool Redo();
    }
}
