using SFML.Graphics;
using Shared.Core;

namespace Shared.Interfaces
{
    public interface IApplicationInstance
    {
        public string DisplayName { get; }

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

        // The update trigger from the parent program for this application to update
        public void OnUpdate(float deltaT);

        // The render trigger from the parent program for this application to render
        public void OnRender(RenderTarget target);

        // Pause the application, keep it loaded but is no longer running
        public void Suspend();

        // Prepare the application to no longer be running, and unload / cleanup as required
        public void Stop();

        // Recover the application from the exception thrown, return true if recovered, false if unable to recover.
        public bool OnException();
    }
}
