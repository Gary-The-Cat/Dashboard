using SFML.Window;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Services
{
    public class EventService : IEventService
    {
        private Func<IApplicationInstance> getActiveApplication;
        private Dictionary<Guid, List<(KeyPressCallbackEventArgs args, Action<KeyboardEventArgs> callback)>> keyPressEvents;
        private Dictionary<Guid, List<(MouseClickCallbackEventArgs args, Action<MouseClickEventArgs> callback)>> mouseClickEvents;
        private Dictionary<Guid, List<Action<MoveMouseEventArgs>>> mouseMoveEvents;

        public EventService(
            Window window, 
            Func<IApplicationInstance> getActiveApplication)
        {
            this.getActiveApplication = getActiveApplication;
            this.keyPressEvents = new Dictionary<Guid, List<(KeyPressCallbackEventArgs, Action<KeyboardEventArgs>)>>();
            this.mouseClickEvents = new Dictionary<Guid, List<(MouseClickCallbackEventArgs args, Action<MouseClickEventArgs> callback)>>();
            this.mouseMoveEvents = new Dictionary<Guid, List<Action<MoveMouseEventArgs>>>();

            this.InitializeKeyboardListener(window);
            this.InitializeMouseClickListener(window);
            this.InitializeMouseMoveListener(window);
        }

        private void InitializeKeyboardListener(Window window)
        {
            window.KeyPressed += (_, e) =>
            {
                var keyboardEventArgs = new KeyboardEventArgs(e);
                var activeApplication = getActiveApplication();

                // Iterate over each of the currently active screens in the application & perform any callbacks
                var screenIds = activeApplication.ScreenManager.GetScreenIds();

                // Check for handling top down (drawn last first, so reversed from the screen Ids list)
                foreach (var screen in screenIds.Reverse())
                {
                    // The event has been handled, we should stop
                    if (keyboardEventArgs.IsHandled)
                    {
                        break;
                    }

                    // This screen is not active, skip it
                    if (!activeApplication.ScreenManager.IsScreenActive(screen))
                    {
                        continue;
                    }

                    // This screen is active but there are no events registered for this screen
                    if (!keyPressEvents.ContainsKey(screen))
                    {
                        continue;
                    }

                    // There are events, and this screen is active, loop through each of its events and call them
                    foreach (var keyboardEvent in keyPressEvents[screen])
                    {
                        // If the event does not match the event arguments
                        if (e.Code != keyboardEvent.args.Key || 
                            keyboardEvent.args.IsCtrlModifierRequired != e.Control || 
                            keyboardEvent.args.IsShiftModifierRequired != e.Shift)
                        {
                            continue;
                        }

                        keyboardEvent.callback?.Invoke(keyboardEventArgs);
                    }
                }
            };
        }

        private void InitializeMouseClickListener(Window window)
        {
            window.MouseButtonPressed += (_, e) =>
            {
                var mouseEventArgs = new MouseClickEventArgs(e);
                var activeApplication = getActiveApplication();

                // Iterate over each of the currently active screens in the application & perform any callbacks
                var screenIds = activeApplication.ScreenManager.GetScreenIds();

                // Check for handling top down (drawn last first, so reversed from the screen Ids list)
                foreach (var screen in screenIds.Reverse())
                {
                    // The event has been handled, we should stop
                    if (mouseEventArgs.IsHandled)
                    {
                        break;
                    }

                    // The screen is not active
                    if (!activeApplication.ScreenManager.IsScreenActive(screen))
                    {
                        continue;
                    }

                    // There are no events set up for this screen
                    if (!mouseClickEvents.ContainsKey(screen))
                    {
                        continue;
                    }

                    // There are events, and this screen is active, loop through each of its events and call them
                    foreach (var mouseEvent in mouseClickEvents[screen])
                    {
                        // If the event does not match the event arguments
                        if (e.Button != mouseEvent.args.Button)
                        {
                            continue;
                        }

                        mouseEvent.callback?.Invoke(mouseEventArgs);
                    }
                }
            };
        }


        private void InitializeMouseMoveListener(Window window)
        {
            window.MouseMoved += (_, e) =>
            {
                var mouseMoveEventArgs = new MoveMouseEventArgs(e);
                var activeApplication = getActiveApplication();

                // Iterate over each of the currently active screens in the application & perform any callbacks
                var screenIds = activeApplication.ScreenManager.GetScreenIds();

                // Check for handling top down (drawn last first, so reversed from the screen Ids list)
                foreach (var screen in screenIds.Reverse())
                {
                    // The event has been handled, we should stop
                    if (mouseMoveEventArgs.IsHandled)
                    {
                        break;
                    }

                    // The screen is not active
                    if (!activeApplication.ScreenManager.IsScreenActive(screen))
                    {
                        continue;
                    }

                    // There are no events set up for this screen
                    if (!mouseMoveEvents.ContainsKey(screen))
                    {
                        continue;
                    }

                    // There are events, and this screen is active, loop through each of its events and call them
                    foreach (var mouseEvent in mouseMoveEvents[screen])
                    {
                        mouseEvent?.Invoke(mouseMoveEventArgs);
                    }
                }
            };
        }

        public void RegisterKeyboardCallback(
           Guid screenId,
           KeyPressCallbackEventArgs eventArgs,
           Action<KeyboardEventArgs> callback)
        {
            if (!this.keyPressEvents.ContainsKey(screenId))
            {
                this.keyPressEvents.Add(screenId, new List<(KeyPressCallbackEventArgs, Action<KeyboardEventArgs>)>());
            }

            this.keyPressEvents[screenId].Add((eventArgs, callback));
        }

        public void RegisterMouseClickCallback(
            Guid screenId,
            MouseClickCallbackEventArgs eventArgs,
            Action<MouseClickEventArgs> callback)
        {
            if (!this.mouseClickEvents.ContainsKey(screenId))
            {
                this.mouseClickEvents.Add(screenId, new List<(MouseClickCallbackEventArgs, Action<MouseClickEventArgs>)>());
            }

            this.mouseClickEvents[screenId].Add((eventArgs, callback));
        }

        public void RegisterMouseMoveCallback(
            Guid screenId,
            Action<MoveMouseEventArgs> callback)
        {
            if (!this.mouseMoveEvents.ContainsKey(screenId))
            {
                this.mouseMoveEvents.Add(screenId, new List<Action<MoveMouseEventArgs>>());
            }

            this.mouseMoveEvents[screenId].Add(callback);

        }

        //public void RegisterJoystickCallback(Window window, uint button, Action callback)
        //{
        //    window.JoystickButtonPressed += (_, e) =>
        //    {
        //        if (!IsActive || e.Button != button)
        //        {
        //            return;
        //        }

        //        callback?.Invoke();
        //    };
        //}
    }
}
