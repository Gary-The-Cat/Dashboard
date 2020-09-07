using SFML.Graphics;
using SFML.Window;
using Shared.CameraTools;
using Shared.ScreenConfig;
using System;

namespace Shared.Core
{
    public class Screen
    {
        public Camera Camera { get; set; }

        public bool IsActive => IsUpdate && IsApplicationActive;

        public bool IsUpdate { get; set; }

        public bool IsDraw { get; set; }

        public bool IsApplicationActive { get; set; }

        public Screen(ScreenConfiguration configuration)
        {
            Camera = new Camera(configuration);

            IsUpdate = true;
            IsDraw = true;
        }

        public void RegisterKeyboardCallback(
            Window window,
            Keyboard.Key key,
            Action callback,
            bool controlModifier = false,
            bool shiftModifier = false)
        {
            window.KeyPressed += (_, e) =>
            {
                if (!IsActive || e.Code != key)
                {
                    return;
                }

                if (controlModifier != e.Control || shiftModifier != e.Shift)
                {
                    return;
                }

                callback?.Invoke();
            };
        }

        public void RegisterMouseClickCallback(Window window, Mouse.Button button, Action<float, float> callback)
        {
            window.MouseButtonPressed += (_, e) =>
            {
                if (!IsActive || e.Button != button)
                {
                    return;
                }

                callback?.Invoke(e.X, e.Y);
            };
        }

        public void RegisterMouseMoveCallback(Window window, Action<float, float> callback)
        {
            window.MouseMoved += (_, e) =>
            {
                if (!IsActive)
                {
                    return;
                }

                callback?.Invoke(e.X, e.Y);
            };
        }

        public void RegisterJoystickCallback(Window window, uint button, Action callback)
        {
            window.JoystickButtonPressed += (_, e) =>
            {
                if (!IsActive || e.Button != button)
                {
                    return;
                }

                callback?.Invoke();
            };
        }

        public virtual void OnUpdate(float deltaT)
        {
            Camera.Update(deltaT);
        }

        public virtual void OnRender(RenderTarget target)
        {

        }

        public virtual void InitializeScreen()
        {

        }

        public virtual void Suspend()
        {
            IsApplicationActive = false;
        }

        public virtual void Resume()
        {
            IsApplicationActive = true;
        }

        public virtual void Start()
        {
            IsApplicationActive = true;
        }

        public void SetInactive()
        {
            IsUpdate = false;
            IsDraw = false;
        }

        public void SetActive()
        {
            IsUpdate = true;
            IsDraw = true;
        }

        public void SetUpdateInactive()
        {
            IsUpdate = false;
        }

        public void SetDrawInactive()
        {
            IsDraw = false;
        }

        public void SetUpdateActive()
        {
            IsUpdate = true;
        }

        public void SetDrawActive()
        {
            IsDraw = true;
        }
    }
}