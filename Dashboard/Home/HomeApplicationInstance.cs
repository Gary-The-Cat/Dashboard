using Dashboard.ProgramLoad;
using Dashboard.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dashboard.Core
{
    public class HomeApplicationInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private List<ApplicationInstanceVisual> applications;

        public HomeApplicationInstance(IApplication application) : base(application)
        {
            this.applications = new List<ApplicationInstanceVisual>();

            this.GoHome = () => application.ApplicationManager.GoHome();

            var applicationTypes = ApplicationLoader.GetApplicationsInstances();

            // Load all the application instances
            for ( int i = 0; i < applicationTypes.Count; i++)
            {
                var type = applicationTypes[i];

                // Try to create an instance of our ApplicationInstance who has a constructor containing requiging our main applicaiton.
                IApplicationInstance applicationInstance;
                try
                {
                    applicationInstance = (IApplicationInstance)Activator.CreateInstance(type, application);
                    applicationInstance.EventService = application.EventService;
                    applicationInstance.NotificationService = application.NotificaitonService;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to load '{type.Name}', with error:\n{e.InnerException}");
                    continue;
                }

                // If the application has not set a thumbnail, use our default.
                if(applicationInstance.Thumbnail == null)
                {
                    Texture texture = new Texture(new Image("Resources\\DemoApp.png"));
                    texture.GenerateMipmap();
                    texture.Smooth = true;

                    applicationInstance.Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
                    {
                        Texture = texture
                    };
                }

                // Hard coded for 2 applications. I had a think but couldn't think of any good logic to scale this to 'n' instances?
                // If you have something plz do add otherwise I'll look for a library.
                var applicationVisual = new ApplicationInstanceVisual(applicationInstance, new SFML.System.Vector2f(300 + 400 * i, 200));

                applications.Add(applicationVisual);
            }
        }

        public string DisplayName => "Home";

        public new void Initialize()
        {
            var homeScreen = new HomeScreen(Application, this, applications);

            AddChildScreen(homeScreen, null);

            SetActiveScreen(homeScreen);

            base.Initialize();
        }
    }
}
