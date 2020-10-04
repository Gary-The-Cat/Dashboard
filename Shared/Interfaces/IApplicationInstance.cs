using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Core.Hierarchy;
using Shared.Interfaces.Services;
using System;

namespace Shared.Interfaces
{
    public interface IApplicationInstance
    {
        public Guid Id { get; }

        public string DisplayName { get; }

        public Vector2f WindowSize { get; }

        public ScreenManager ScreenManager { get; set; }

        public RectangleShape Thumbnail { get; set; }

        public IApplication Application { get; set; }

        public IEventService EventService { get; set; }

        public INotificationService NotificationService { get; set; }

        public bool IsInitialized { get; }

        public bool IsActive { get; }

        // Code that should run the first time that the application is launched in a session
        public void Initialize();

        // The code that should run every time the application is set to running within a session
        public void Start();

        // Pause the application, keep it loaded but is no longer running
        public void Suspend();

        // Prepare the application to no longer be running, and unload / cleanup as required
        public void Stop();

        // The user has pressed the global 'back' key.
        public Action GoBack { get; set; }

        // The user has pressed the global 'back' key.
        public Action GoHome { get; set; }

        // Recover the application from the exception thrown, return true if recovered, false if unable to recover.
        public bool OnException();

        public void OnUpdate(float deltaT);

        public void OnRender(RenderTarget target);

        public void AddChildScreen(Screen screen, Screen parentScreen);

        public void SetActiveScreen(Screen screen);

        public void RemoveScreen(Screen screen);

        public View DefaultView => Application.GetDefaultView();
    }
}
