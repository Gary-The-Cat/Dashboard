using static SFML.Window.Keyboard;

namespace Shared.Events.CallbackArgs
{
    public class KeyPressCallbackEventArgs
    {
        public KeyPressCallbackEventArgs(Key key, bool isCtrlRequired = false, bool isShiftRequired = false)
        {
            Key = key;
            IsCtrlModifierRequired = isCtrlRequired;
            IsShiftModifierRequired = isShiftRequired;
        }

        public bool IsCtrlModifierRequired { get; set; }

        public bool IsShiftModifierRequired { get; set; }

        public Key Key { get; set; }
    }
}
