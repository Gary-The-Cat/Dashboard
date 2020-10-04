using SFML.Graphics;
using Shared.CameraTools;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.Interfaces;
using System;

namespace Shared.Core
{
    public class Screen
    {
        public Guid Id { get; set; }

        public Guid StackedParentId { get; set; }

        public bool IsStackedChild => StackedParentId != default;

        public Camera Camera { get; set; }

        public bool IsActive => IsUpdate && IsApplicationActive;

        public bool IsUpdate { get; set; }

        public bool IsDraw { get; set; }

        public bool IsApplicationActive { get; set; }

        public IApplication Application { get; set; }

        public IApplicationInstance ParentApplication { get; set; }

        public Screen(IApplication application, IApplicationInstance applicationInstance)
        {
            Camera = new Camera(application.Configuration);
            ParentApplication = applicationInstance;
            Application = application;

            IsUpdate = true;
            IsDraw = true;

            Id = Guid.NewGuid();
        }

        public void RegisterMouseClickCallback(MouseClickCallbackEventArgs eventArgs, Action<MouseClickEventArgs> callback) =>
            ParentApplication.EventService.RegisterMouseClickCallback(this.Id, eventArgs, callback);

        public void RegisterJoystickCallback(JoystickCallbackEventArgs eventArgs, Action<JoystickEventArgs> callback) =>
            ParentApplication.EventService.RegisterJoystickButtonCallback(this.Id, eventArgs, callback);

        public void RegisterMouseMoveCallback(Action<MoveMouseEventArgs> callback) =>
            ParentApplication.EventService.RegisterMouseMoveCallback(this.Id, callback);

        public void RegisterMouseWheelScrollCallback(Action<MouseWheelScrolledEventArgs> callback) =>
            ParentApplication.EventService.RegisterMouseWheelScrollCallback(this.Id, callback);

        public void RegisterKeyboardCallback(KeyPressCallbackEventArgs eventArgs, Action<KeyboardEventArgs> callback) =>
            ParentApplication.EventService.RegisterKeyboardCallback(this.Id, eventArgs, callback);

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

        public virtual void SetInactive()
        {
            IsUpdate = false;
            IsDraw = false;
        }

        public virtual void SetActive()
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