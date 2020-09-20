using static SFML.Window.Mouse;

namespace Shared.Events.CallbackArgs
{
    public class MouseClickCallbackEventArgs
    {
        public Button Button { get; set; }

        public MouseClickCallbackEventArgs(Button button)
        {
            Button = button;
        }
    }
}
