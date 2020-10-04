using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using System;

namespace Shared.Interfaces.Services
{
    public interface IEventService
    {
        public void RegisterMouseClickCallback(
            Guid screenId,
            MouseClickCallbackEventArgs eventArgs,
            Action<MouseClickEventArgs> callback);

        public void RegisterMouseWheelScrollCallback(
            Guid screenId,
            Action<MouseWheelScrolledEventArgs> callback);

        public void RegisterMouseMoveCallback(
            Guid screenId,
            Action<MoveMouseEventArgs> callback);

        public void RegisterKeyboardCallback(
           Guid screenId,
           KeyPressCallbackEventArgs eventArgs,
           Action<KeyboardEventArgs> callback);

        public void RegisterJoystickButtonCallback(
           Guid screenId,
           JoystickCallbackEventArgs eventArgs,
           Action<JoystickEventArgs> callback);
    }
}
