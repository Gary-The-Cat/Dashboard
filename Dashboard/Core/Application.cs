using Ninject;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.CameraTools;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using Shared.Notifications;
using Shared.ScreenConfig;
using Shared.Services;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Dashboard.Core
{
    public class Application : IApplication
    {
        public Window Window => window;

        private RenderWindow window;

        public ScreenConfiguration Configuration { get; set; }

        public IApplicationManager ApplicationManager => AppManager;

        private ApplicationManager AppManager { get; set; }

        public IApplicationInstance ActiveApplication => AppManager.ActiveApplication;

        private IApplicationInstance HomeApplication => AppManager.HomeApplication;

        public IEventService EventService { get; set; }

        public INotificationService NotificaitonService { get; set; }

        private StandardKernel kernel;

        public Application(RenderWindow window, ScreenConfiguration configuration)
        {
            this.window = window;

            Configuration = configuration;

            ConfigureKernel();

            NotificaitonService = kernel.Get<INotificationService>();

            EventService = kernel.Get<IEventService>();

            AppManager = new ApplicationManager(this);

            AppManager.HomeApplication = new HomeApplicationInstance(this);

            AppManager.HomeApplication.EventService = EventService;

            AppManager.SetActiveApplication(HomeApplication);

        }

        private void ConfigureKernel()
        {
            Func<IApplicationInstance> getActiveApplication = () => ActiveApplication;
            Func<View> getDefaultView = GetDefaultView;
            Func<Vector2u> getWindowSize = () => window.Size;

            kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            kernel.Bind<IEventService>().To<EventService>()
                .InSingletonScope()
                .WithConstructorArgument("window", window)
                .WithConstructorArgument("getActiveApplication", getActiveApplication);

            kernel.Bind<INotificationService>().To<NotificationService>()
                .InSingletonScope()
                .WithConstructorArgument("getDefaultView", getDefaultView)
                .WithConstructorArgument("getWindowSize", getWindowSize);

        }

        public void Start()
        {
            Initialise();

            // Register the closed callback
            window.Closed += (sender, e) => window.Close();

            // Run
            Run();

            ActiveApplication.Stop();
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
                    AppManager.OnUpdate(0.016f);

                    // Draw, not sure extent of logic to do in this class
                    AppManager.OnRender(window);

                    // Display updated frame
                    window.Display();
                }
            }
            catch (Exception e)
            {
                // Log the error
                Debug.WriteLine($"Application '{ActiveApplication.DisplayName}' exited with exception\n{e.InnerException}");

                // Cleanup the application & perform recovery where possible
                if (ActiveApplication.OnException())
                {
                    AppManager.SetActiveApplication(ActiveApplication);
                }
                else
                {
                    AppManager.SetActiveApplication(ActiveApplication);
                }

                Run();
            }
        }

        private void Initialise()
        {
            if (window == null)
            {
                window = new RenderWindow(new VideoMode(1280, 720), "Application");
            }
        }

        public View GetDefaultView()
        {
            return new Camera(Configuration).GetView();
        }
    }
}