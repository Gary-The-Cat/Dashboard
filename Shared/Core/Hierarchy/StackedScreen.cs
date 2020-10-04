using SFML.Graphics;
using Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Core.Hierarchy
{
    public class StackedScreen : Screen
    {
        private List<Screen> screens;

        public IEnumerable<Screen> ReversedScreens => screens.AsEnumerable().Reverse();

        public StackedScreen(IApplication application, IApplicationInstance applicationInstance) 
            : base(application, applicationInstance)
        {
            screens = new List<Screen>();
        }

        public void AddScreen(Screen screen)
        {
            // Set the Id for the stacked/container for this screen
            screen.StackedParentId = this.Id;

            screens.Add(screen);
        }

        public override void InitializeScreen()
        {
            base.InitializeScreen();

            screens.ForEach(screen => screen.InitializeScreen());
        }

        public override void OnUpdate(float deltaT)
        {
            base.OnUpdate(deltaT);

            screens.ForEach(screen => screen.OnUpdate(deltaT));
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);

            screens.ForEach(screen => screen.OnRender(target));
        }
    }
}
