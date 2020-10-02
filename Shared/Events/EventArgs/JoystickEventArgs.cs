using SFML.Window;
using Shared.Events.CallbackArgs;

namespace Shared.Events.EventArgs
{
    public class JoystickEventArgs
    {
        public JoystickEventArgs(JoystickButtonEventArgs args)
        {
            this.Key = args.Button;
        }

        public uint Key { get; set; }

        public bool IsHandled { get; set; }
    }
}
