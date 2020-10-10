using Dashboard.ProgramLoad;
using Dashboard.Screens;
using Ninject;
using Ninject.Parameters;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dashboard.Core
{
    public class HomeApplicationInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private List<ApplicationInstanceVisual> applications;

        private IApplicationService appService;

        public HomeApplicationInstance(IApplicationService appService, IApplicationManager appManager)
        {
            IApplication application = null;

            this.appService = appService;

            this.applications = new List<ApplicationInstanceVisual>();

            this.GoHome = () => appManager.GoHome();

            var applicationTypes = ApplicationLoader.GetApplicationsInstances();

            // Load all the application instances
            for ( int i = 0; i < applicationTypes.Count; i++)
            {
                var type = applicationTypes[i];
                // Try to create an instance of our ApplicationInstance who has a constructor containing requiging our main applicaiton.
                IApplicationInstance applicationInstance;
                try
                {
                    applicationInstance = (IApplicationInstance)appService.Kernel.Get(type);

                    ConfigureBackBehaviour(applicationInstance, appManager);
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

        private void ConfigureBackBehaviour(
            IApplicationInstance applicationInstance, 
            IApplicationManager appManager)
        {
            applicationInstance.GoHome = () => appManager.GoHome();

            applicationInstance.GoBack = () =>
            {
                if (applicationInstance.ScreenManager.CurrentLayer > 0)
                {
                    applicationInstance.ScreenManager.GoBack();
                }
                else
                {
                    appManager.GoHome();
                }
            };
        }

        public string DisplayName => "Home";

        public new void Initialize()
        {
            var homeScreen = appService.Kernel.Get<HomeScreen>(
                new ConstructorArgument("applicationInstances", applications));

            AddChildScreen(homeScreen);

            base.Initialize();
        }
    }
}
