using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Interfaces;
using Shared.Menus;
using System;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakerHudScreen : Screen
    {
        private IApplication application;

        private Button stateText;

        private MapEditState state;

        public MapMakerHudScreen(IApplication application)
        {
            this.application = application;

            application.Window.KeyPressed += OnKeyPress;

            var stateTextPosition = new Vector2f(application.Window.Size.X - 100, application.Window.Size.Y - 40);

            stateText = new Button("Line Creation", stateTextPosition);
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (IsActive)
            {

            }
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
        }

        public override void OnRender(RenderTarget target)
        {
            target.SetView(application.GetDefaultView());

            stateText.OnRender(target);
        }
    }
}
