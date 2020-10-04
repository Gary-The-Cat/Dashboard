using SFML.Graphics;
using SFML.System;
using Shared.Core.Hierarchy;
using Shared.Interfaces;
using System;
using System.Diagnostics;

namespace Shared.Core
{
    public class ApplicationInstanceBase
    {
        public ApplicationInstanceBase(IApplication application)
        {
            Id = Guid.NewGuid();
            ScreenManager = new ScreenManager();
            Application = application;

            this.GoHome = () => application.ApplicationManager.GoHome();

            GoBack = () =>
            {
                if(this.ScreenManager.CurrentLayer > 0)
                {
                    this.ScreenManager.GoBack();
                }
                else
                {
                    application.ApplicationManager.GoHome();
                }
            };

        }

        public virtual void Stop()
        {
            Debug.WriteLine($"Stop: {GetType().Name}");
            this.IsActive = false;
        }

        public virtual void Start()
        {
            Debug.WriteLine($"Start: {GetType().Name}");
            this.IsActive = true;
        }

        public virtual void Initialize()
        {
            Debug.WriteLine($"Initialize: {GetType().Name}");
            this.IsInitialized = true;
            ScreenManager.Start();
        }

        public virtual void Suspend()
        {
            Debug.WriteLine($"Suspend: {GetType().Name}");
            this.IsActive = false;
        }

        public virtual bool OnException()
        {
            Debug.WriteLine($"Exception thrown and unhandled by: {GetType().Name}");
            return false;
        }

        public virtual void OnUpdate(float deltaT)
        {
            ScreenManager.OnUpdate(deltaT);
        }

        public virtual void OnRender(RenderTarget target)
        {
            ScreenManager.OnRender(target);
        }

        public Action GoBack { get; set; }

        public Action GoHome { get; set; }

        public void AddChildScreen(Screen screen, Screen parentScreen) => ScreenManager.AddChildScreen(screen, parentScreen);

        public void SetActiveScreen(Screen screen) => ScreenManager.SetActiveScreen(screen);

        public void RemoveScreen(Screen screen) => ScreenManager.RemoveScreen(screen);

        public Guid Id { get; set; }

        public IApplication Application { get; set; }

        public RectangleShape Thumbnail { get; set; }

        public Vector2f WindowSize { get; set; }

        public bool IsInitialized { get; internal set; }

        public bool IsActive { get; internal set; }

        public ScreenManager ScreenManager { get; set; }

        public IEventService EventService { get; set; }

        public INotificationService NotificationService { get; set; }
    }
}
