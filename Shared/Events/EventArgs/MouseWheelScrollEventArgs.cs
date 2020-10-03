using SFML.Window;

namespace Shared.Events.EventArgs
{
    public class MouseWheelScrolledEventArgs
    {
        public MouseWheelScrolledEventArgs(MouseWheelScrollEventArgs args)
        {
            this.Args = args;
        }

        public MouseWheelScrollEventArgs Args { get; set; }

        public bool IsHandled { get; set; }
    }
}
