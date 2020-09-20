namespace Shared.Events.EventArgs
{
    public class MoveMouseEventArgs
    {
        public MoveMouseEventArgs(SFML.Window.MouseMoveEventArgs args)
        {
            Args = args;
            IsHandled = false;
        }

        public SFML.Window.MouseMoveEventArgs Args { get; set; }

        public bool IsHandled { get; set; }
    }
}
