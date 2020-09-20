using SFML.Window;

namespace Shared.Events.EventArgs
{
    public class MouseClickEventArgs
    {
        public bool IsHandled { get; set; }

        public MouseButtonEventArgs Args { get; set; }

        public MouseClickEventArgs(MouseButtonEventArgs args)
        {
            this.Args = args;
            this.IsHandled = false;
        }
    }
}
