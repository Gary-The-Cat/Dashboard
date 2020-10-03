using SFML.Graphics;
using Shared.Interfaces;
using Shared.ScreenConfig;
using System.Collections.Generic;

namespace Shared.Core.Hierarchy
{
    public class StackedScreen : Screen
    {
        private List<Screen> screens;

        public StackedScreen(ScreenConfiguration configuration, IApplicationInstance applicationInstance) 
            : base(configuration, applicationInstance)
        {
            screens = new List<Screen>();
        }

        public void AddScreen(Screen screen)
        {
            screens.Add(screen);
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

        public override void SetActive()
        {
            base.SetActive();

            screens.ForEach(screen => screen.SetActive());
        }

        public override void SetInactive()
        {
            base.SetInactive();

            screens.ForEach(screen => screen.SetInactive());
        }
    }
}
