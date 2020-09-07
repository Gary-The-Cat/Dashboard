using SFML.Graphics;
using Shared.Core;
using System;

namespace Shared.Interfaces
{
    public interface IApplicationInstance
    {
        public Guid Id { get; }

        public string DisplayName { get; }

        public ScreenManager ScreenManager { get; set; }

        public RectangleShape Thumbnail { get; set; }

        public IApplication Application { get; set; }

        public Screen Screen { get; set; }

        public RenderWindow RenderWindow { get; set; }

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

        // Recover the application from the exception thrown, return true if recovered, false if unable to recover.
        public bool OnException();

        public void OnUpdate(float deltaT);

        public void OnRender(RenderTarget target);
    }
}
