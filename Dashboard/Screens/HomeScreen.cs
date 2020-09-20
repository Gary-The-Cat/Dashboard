using Dashboard.Home;
using SFML.Graphics;
using SFML.Window;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Interfaces;
using System;
using System.Collections.Generic;

namespace Dashboard.Screens
{
    /// <summary>
    /// Home Screen - This is the landing page for our brand new application! We can make this as nice as we want, but 
    /// for now all it does is show the applications on the screen, and start one that gets clicked.
    /// </summary>
    public class HomeScreen : Screen
    {
        private IApplication application;

        private List<ApplicationInstanceVisual> applications;

        private Action<IApplicationInstance> setActiveApplication;

        private ApplicationDashboard applicationDashboard;

        private IApplicationInstance selectedApplication => applicationDashboard.SelectedApplication.ApplicationInstance;

        public HomeScreen(
            IApplication application,
            IApplicationInstance applicationInstance,
            Action<IApplicationInstance> setActiveApplication,
            List<ApplicationInstanceVisual> applicationInstances) : base(application.Configuration, applicationInstance)
        {
            this.application = application;
            this.setActiveApplication = setActiveApplication;
            this.applications = applicationInstances;
            this.applicationDashboard = new ApplicationDashboard(
                applicationInstances, 
                application);

            this.RegisterKeyboardCallback(
                new KeyPressCallbackEventArgs(Keyboard.Key.Enter),
                (_) => this.setActiveApplication(selectedApplication));
        }

        public override void OnUpdate(float dt)
        {
            this.applicationDashboard.Update(dt);
        }

        public override void OnRender(RenderTarget target)
        {
            this.applicationDashboard.Draw(target);
        }

        public override void Suspend()
        {
            this.applicationDashboard.IsActive = false;
        }

        public override void Resume()
        {
            this.applicationDashboard.IsActive = true;
        }
    }
}
