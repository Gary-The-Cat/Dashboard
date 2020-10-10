using Dashboard.Home;
using Ninject;
using Ninject.Parameters;
using SFML.Graphics;
using SFML.Window;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using System.Collections.Generic;

namespace Dashboard.Screens
{
    /// <summary>
    /// Home Screen - This is the landing page for our brand new application! We can make this as nice as we want, but 
    /// for now all it does is show the applications on the screen, and start one that gets clicked.
    /// </summary>
    public class HomeScreen : Screen
    {
        private List<ApplicationInstanceVisual> applications;

        private ApplicationDashboard applicationDashboard;

        private IApplicationInstance selectedApplication => applicationDashboard.SelectedApplication.ApplicationInstance;

        public HomeScreen(
            IApplicationManager applicationManager,
            IEventService eventService,
            IApplicationService appService,
            List<ApplicationInstanceVisual> applicationInstances) : base()
        {
            this.applications = applicationInstances;
            this.applicationDashboard = appService.Kernel.Get<ApplicationDashboard>(
                new ConstructorArgument("applications", applicationInstances));

            eventService.RegisterKeyboardCallback(Id, new KeyPressCallbackEventArgs(Keyboard.Key.Left), applicationDashboard.LeftKeyPressed);
            eventService.RegisterKeyboardCallback(Id, new KeyPressCallbackEventArgs(Keyboard.Key.Right), applicationDashboard.RightKeyPressed);
            eventService.RegisterMouseClickCallback(Id, new MouseClickCallbackEventArgs(Mouse.Button.Left), applicationDashboard.OnMouseClick);
            eventService.RegisterMouseWheelScrollCallback(Id, applicationDashboard.OnMouseWheelMove);

            eventService.RegisterKeyboardCallback(
                Id,
                new KeyPressCallbackEventArgs(Keyboard.Key.Enter),
                (_) => applicationManager.SetActiveApplication(selectedApplication));
        }

        public override void OnUpdate(float dt)
        {
            this.applicationDashboard.Update(dt);
        }

        public override void OnRender(RenderTarget target)
        {
            this.applicationDashboard.Draw(target);
        }
    }
}
