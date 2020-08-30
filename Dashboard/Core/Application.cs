using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.CameraTools;
using Shared.Core;
using Shared.Interfaces;
using Shared.ScreenConfig;
using System;
using System.Diagnostics;

namespace Dashboard.Core
{
    public class Application : IApplication
    {
        public Window Window => window;

        private RenderWindow window;

        private IApplicationInstance homeApplicationinstance;

        private IApplicationInstance activeApplicationInstance;

        public Screen Screen { get; set; }

        public ScreenConfiguration Configuration { get; set; }

        public Application(RenderWindow window, ScreenConfiguration configuration)
        {
            this.window = window;

            this.Configuration = configuration;

            homeApplicationinstance = new HomeApplicationInstance(this, SetActiveApplicationInstance);

            activeApplicationInstance = homeApplicationinstance;
        }

        public void Start()
        {
            Initialise();

            // Register the closed callback
            window.Closed += (sender, e) => window.Close();

            // Run
            this.Run();

            activeApplicationInstance.Stop();
        }

        public void Run()
        {
            // Main game loop
            try
            {
                var timer = new Clock();

                while (window.IsOpen)
                {
                    window.DispatchEvents();

                    // Clear the previous frame
                    window.Clear(new Color(0xe9, 0xe9, 0xe9));

                    // Update, not sure extent of logic to do in this class
                    OnUpdate(0.016f);

                    // Draw, not sure extent of logic to do in this class
                    OnRender(window);

                    // Display updated frame
                    window.Display();
                }
            }
            catch (Exception e)
            {
                // Log the error
                Debug.WriteLine($"Application '{activeApplicationInstance.DisplayName}' exited with exception\n{e.InnerException}");

                // Cleanup the application & perform recovery where possible
                if (this.activeApplicationInstance.OnException())
                {
                    this.SetActiveApplicationInstance(this.activeApplicationInstance);
                }
                else
                {
                    this.SetActiveApplicationInstance(this.homeApplicationinstance);
                }

                this.Run();
            }
        }

        public void Stop()
        {
            window.Close();
        }

        private void SetActiveApplicationInstance(IApplicationInstance applicationInstance)
        {
            // Suspend the current application instance
            this.activeApplicationInstance.Suspend();

            // Probably need to think of a nicer way to do this, but for now passing in null brings us back to our home screen
            this.activeApplicationInstance = applicationInstance ?? homeApplicationinstance;
                        
            // Ensure that the application has been initialized
            if (this.activeApplicationInstance.IsInitialized)
            {
                this.activeApplicationInstance.Initialize();
            }

            // Every time an application becomes active, we call start, regardless of initialization status.
            this.activeApplicationInstance.Start();
            
        }

        private void Initialise()
        {
            if (window == null)
            {
                window = new RenderWindow(new VideoMode(1280, 720), "Application");
            }

            this.activeApplicationInstance.Initialize();

            // TODO initialise keyboard/mouse objects once they exist.
        }

        private void OnUpdate(float deltaT)
        {
            // Check if the user wants to return home. I like this being above the logic of the currently active application.
            // But perhaps not this high, not sure if we need a level lower than this that is in charge of managing the
            // application instances. This could be a single layer that could provide pause / playback functionality
            // etc to any of the games/simulations we make.
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
                this.SetActiveApplicationInstance(null);

                // We always reset the camera to the default view
                this.window.SetView(GetDefaultView());
            }

            activeApplicationInstance.OnUpdate(deltaT);
        }

        private void OnRender(RenderTarget target)
        {
            activeApplicationInstance.OnRender(target);
        }

        public View GetDefaultView()
        {
            return new Camera(Configuration.SinglePlayer, Configuration).GetView();
        }
    }
}