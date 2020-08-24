using Dashboard.Home;
using SFML.Graphics;
using SFML.Window;
using Shared.Core;
using Shared.ExtensionMethods;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public HomeScreen(
            IApplication application,
            Action<IApplicationInstance> setActiveApplication,
            List<ApplicationInstanceVisual> applicationInstances)
        {
            this.application = application;
            this.setActiveApplication = setActiveApplication;
            this.applications = applicationInstances;
            this.applicationDashboard = new ApplicationDashboard(
                applicationInstances, 
                application);

            this.application.Window.KeyPressed += OnKeyPress;
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Enter)
            {
                this.setActiveApplication(applicationDashboard.SelectedApplication.ApplicationInstance);
            }
        }

        public override void OnUpdate(float dt)
        {
            this.applicationDashboard.Update(dt);
        }

        public override void OnRender(RenderTarget target)
        {
            this.applicationDashboard.Draw(target);
        }

        public override void OnExit()
        {
            base.OnExit();

            this.applicationDashboard.IsActive = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            this.applicationDashboard.IsActive = true;
        }
    }
}
