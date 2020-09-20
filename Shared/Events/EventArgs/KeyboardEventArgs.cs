using SFML.Window;

namespace Shared.Events.EventArgs
{
    public class KeyboardEventArgs
    {
        public bool IsHandled { get; set; }

        public KeyEventArgs Args { get; set; }

        public KeyboardEventArgs(KeyEventArgs args)
        {
            this.Args = args;
            this.IsHandled = false;
        }
    }
}
