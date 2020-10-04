using SFML.Window;
using Shared.Core.Hierarchy;
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
        private Dictionary<Guid, List<(JoystickCallbackEventArgs args, Action<JoystickEventArgs> callback)>> joystickButtonEvents;
        private Dictionary<Guid, List<(MouseClickCallbackEventArgs args, Action<MouseClickEventArgs> callback)>> mouseClickEvents;
        private Dictionary<Guid, List<Action<MoveMouseEventArgs>>> mouseMoveEvents;
        private Dictionary<Guid, List<Action<MouseWheelScrolledEventArgs>>> mouseScrollEvents;

        public EventService(
            Window window, 
            Func<IApplicationInstance> getActiveApplication)
        {
            this.getActiveApplication = getActiveApplication;
            this.keyPressEvents = new Dictionary<Guid, List<(KeyPressCallbackEventArgs, Action<KeyboardEventArgs>)>>();
            this.joystickButtonEvents = new Dictionary<Guid, List<(JoystickCallbackEventArgs args, Action<JoystickEventArgs> callback)>>();
            this.mouseClickEvents = new Dictionary<Guid, List<(MouseClickCallbackEventArgs args, Action<MouseClickEventArgs> callback)>>();
            this.mouseMoveEvents = new Dictionary<Guid, List<Action<MoveMouseEventArgs>>>();
            this.mouseScrollEvents = new Dictionary<Guid, List<Action<MouseWheelScrolledEventArgs>>>();

            this.InitializeKeyboardListener(window);
            this.InitializeJoystickButtonListener(window);
            this.InitializeMouseClickListener(window);
            this.InitializeMouseMoveListener(window);
            this.InitializeMouseScrollListener(window);
        }

        private void InitializeKeyboardListener(Window window)
        {
            window.KeyPressed += (_, e) =>
            {
                var keyboardEventArgs = new KeyboardEventArgs(e);
                var activeApplication = getActiveApplication();

                // Iterate over each of the currently active screens in the application & perform any callbacks
                var activeScreen = activeApplication.ScreenManager.ActiveScreen;

                if(activeScreen is StackedScreen stackedScreen)
                {
                    foreach (var screen in stackedScreen.ReversedScreens)
                    {
                        ProcessKeyEvent(screen.Id, e, keyboardEventArgs);

                        if (keyboardEventArgs.IsHandled)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    ProcessKeyEvent(activeScreen.Id, e, keyboardEventArgs);
                }
            };
        }

        private void ProcessKeyEvent(Guid screenId, KeyEventArgs e, KeyboardEventArgs keyboardEventArgs)
        {
            // This screen is active but there are no events registered for this screen
            if (!keyPressEvents.ContainsKey(screenId))
            {
                return;
            }

            // There are events, and this screen is active, loop through each of its events and call them
            foreach (var keyboardEvent in keyPressEvents[screenId])
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

        private void InitializeMouseClickListener(Window window)
        {
            window.MouseButtonPressed += (_, e) =>
            {
                var mouseEventArgs = new MouseClickEventArgs(e);
                var activeApplication = getActiveApplication();

                // Iterate over each of the currently active screens in the application & perform any callbacks
                var activeScreen = activeApplication.ScreenManager.ActiveScreen;                

                if (activeScreen is StackedScreen stackedScreen)
                {
                    foreach (var screen in stackedScreen.ReversedScreens)
                    {
                        ProcessMouseClickEvent(screen.Id, e, mouseEventArgs);

                        if (mouseEventArgs.IsHandled)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    ProcessMouseClickEvent(activeScreen.Id, e, mouseEventArgs);
                }
            };
        }

        private void ProcessMouseClickEvent(Guid screenId, MouseButtonEventArgs e, MouseClickEventArgs mouseEventArgs)
        {
            // There are no events set up for this screen
            if (!mouseClickEvents.ContainsKey(screenId))
            {
                return;
            }

            // There are events, and this screen is active, loop through each of its events and call them
            foreach (var mouseEvent in mouseClickEvents[screenId])
            {
                // If the event does not match the event arguments
                if (e.Button != mouseEvent.args.Button)
                {
                    continue;
                }

                mouseEvent.callback?.Invoke(mouseEventArgs);
            }
        }


        private void InitializeMouseMoveListener(Window window)
        {
            window.MouseMoved += (_, e) =>
            {
                var mouseMoveEventArgs = new MoveMouseEventArgs(e);
                var activeApplication = getActiveApplication();

                // Iterate over each of the currently active screens in the application & perform any callbacks
                var activeScreen = activeApplication.ScreenManager.ActiveScreen;

                if(activeScreen is StackedScreen stackedScreen)
                {
                    foreach (var screen in stackedScreen.ReversedScreens)
                    {
                        ProcessMouseMoveEvent(screen.Id, mouseMoveEventArgs);

                        if (mouseMoveEventArgs.IsHandled)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    ProcessMouseMoveEvent(activeScreen.Id, mouseMoveEventArgs);
                }
            };
        }

        private void ProcessMouseMoveEvent(Guid screenId, MoveMouseEventArgs mouseMoveEventArgs)
        {
            // There are no events set up for this screen
            if (!mouseMoveEvents.ContainsKey(screenId))
            {
                return;
            }

            // There are events, and this screen is active, loop through each of its events and call them
            foreach (var mouseEvent in mouseMoveEvents[screenId])
            {
                mouseEvent?.Invoke(mouseMoveEventArgs);
            }
        }

        private void InitializeMouseScrollListener(Window window)
        {
            window.MouseWheelScrolled += (_, e) =>
            {
                var mouseScrolledEventArgs = new MouseWheelScrolledEventArgs(e);
                var activeApplication = getActiveApplication();

                // Iterate over each of the currently active screens in the application & perform any callbacks
                var activeScreen = activeApplication.ScreenManager.ActiveScreen;

                if (activeScreen is StackedScreen stackedScreen)
                {
                    foreach (var screen in stackedScreen.ReversedScreens)
                    {
                        ProcessMouseScrollEvent(screen.Id, mouseScrolledEventArgs);

                        if (mouseScrolledEventArgs.IsHandled)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    ProcessMouseScrollEvent(activeScreen.Id, mouseScrolledEventArgs);
                }
            };
        }

        private void ProcessMouseScrollEvent(Guid screenId, MouseWheelScrolledEventArgs mouseScrolledEventArgs)
        {
            // There are no events set up for this screen
            if (!mouseScrollEvents.ContainsKey(screenId))
            {
                return;
            }

            // There are events, and this screen is active, loop through each of its events and call them
            foreach (var mouseEvent in mouseScrollEvents[screenId])
            {
                mouseEvent?.Invoke(mouseScrolledEventArgs);
            }
        }

        private void InitializeJoystickButtonListener(Window window)
        {
            window.JoystickButtonPressed += (_, e) =>
            {
                var joystickEventArgs = new JoystickEventArgs(e);
                var activeApplication = getActiveApplication();

                // Iterate over each of the currently active screens in the application & perform any callbacks
                var screenId = activeApplication.ScreenManager.ActiveScreen.Id;

                // There are no events set up for this screen
                if (!joystickButtonEvents.ContainsKey(screenId))
                {
                    return;
                }

                // There are events, and this screen is active, loop through each of its events and call them
                foreach (var joystickButtonEvent in joystickButtonEvents[screenId])
                {
                    // If the event does not match the event arguments
                    if (e.Button != joystickButtonEvent.args.Key)
                    {
                        continue;
                    }

                    joystickButtonEvent.callback?.Invoke(joystickEventArgs);
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

        public void RegisterMouseWheelScrollCallback(
            Guid screenId,
            Action<MouseWheelScrolledEventArgs> callback)
        {
            if (!this.mouseScrollEvents.ContainsKey(screenId))
            {
                this.mouseScrollEvents.Add(screenId, new List<Action<MouseWheelScrolledEventArgs>>());
            }

            this.mouseScrollEvents[screenId].Add(callback);
        }

        public void RegisterJoystickButtonCallback(Guid screenId, JoystickCallbackEventArgs eventArgs, Action<JoystickEventArgs> callback)
        {
            if (!this.joystickButtonEvents.ContainsKey(screenId))
            {
                this.joystickButtonEvents.Add(screenId, new List<(JoystickCallbackEventArgs, Action<JoystickEventArgs>)>());
            }

            this.joystickButtonEvents[screenId].Add((eventArgs, callback));
        }
    }
}
