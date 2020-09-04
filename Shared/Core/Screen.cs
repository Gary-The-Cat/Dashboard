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

        public Func<bool> IsActive { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsDraw { get; set; }

        public Screen(ScreenConfiguration configuration)
        {
            Camera = new Camera(configuration);

            IsUpdate = true;
            IsDraw = true;
        }

        public void RegisterKeyboardCallback(Window window, Keyboard.Key key, Action callback)
        {
            window.KeyPressed += (_, e) =>
            {
                if (!IsActive() || e.Code != key)
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
                if (!IsActive() || e.Button != button)
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
                if (!IsActive())
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
                if (!IsActive() || e.Button != button)
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

        }

        public virtual void Resume()
        {

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