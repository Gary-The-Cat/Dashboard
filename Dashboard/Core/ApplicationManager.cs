using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Core.Hierarchy;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using Shared.ScreenConfig;

namespace Dashboard.Core
{
    public class ApplicationManager : IApplicationManager
    {
        public ApplicationManager(
            Application application,
            INotificationService notificationService)
        {
            this.application = application;
            this.notificationService = notificationService;
            this.application.Window.KeyPressed += OnKeyPressed;
        }

        public IApplicationInstance HomeApplication { get; set; }

        public IApplicationInstance ActiveApplication { get; private set; }

        internal ScreenManager screenManager => ActiveApplication.ScreenManager;

        private readonly Application application;

        private INotificationService notificationService;

        private RenderWindow Window => (RenderWindow)application.Window;

        public void OnUpdate(float deltaT)
        {
            ActiveApplication.OnUpdate(deltaT);

            notificationService.OnUpdate(deltaT);
        }

        public void OnRender(RenderTarget target)
        {
            ActiveApplication.OnRender(target);

            notificationService.OnRender(target);
        }

        public void SetActiveApplication(IApplicationInstance application)
        {
            if (ActiveApplication != null)
            {
                // Suspend all screen activity - event listeners
                ActiveApplication.Suspend();
            }

            // Probably need to think of a nicer way to do this, but for now passing in null brings us back to our home screen
            ActiveApplication = application;

            // Ensure that the application has been initialized
            if (!ActiveApplication.IsInitialized)
            {
                ActiveApplication.Initialize();
            }
            else
            {
                // Every time an application becomes active, we call start, regardless of initialization status.
                ActiveApplication.Start();
            }
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {            
            // Check if the user wants to return home. I like this being above the logic of the currently active application.
            // But perhaps not this high, not sure if we need a level lower than this that is in charge of managing the
            // application instances. This could be a single layer that could provide pause / playback functionality
            // etc to any of the games/simulations we make.
            if (e.Code == Keyboard.Key.Escape)
            {
                ActiveApplication.GoBack();

                // We always reset the camera to the default view
                Window.SetView(application.GetDefaultView());
            }
        }

        public void GoHome()
        {
            ActiveApplication = HomeApplication;
        }

        public Vector2u GetWindowSize()
        {
            return Window.Size;
        }

        public View GetDefaultView()
        {
            return application.GetDefaultView();
        }

        public void SetHomeApplication(IApplicationInstance homeApplication)
        {
            this.HomeApplication = homeApplication;

            SetActiveApplication(homeApplication);
        }

        public ScreenConfiguration GetScreenConfiguration()
        {
            return application.Configuration;
        }

        public void AddChildScreen(Screen screen)
        {
            ActiveApplication.AddChildScreen(screen);
        }

        public void SetActiveScreen(Screen screen)
        {
            ActiveApplication.SetActiveScreen(screen);
        }
    }
}
