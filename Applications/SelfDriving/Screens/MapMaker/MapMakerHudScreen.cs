using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Interfaces;
using Shared.Menus;
using System;
using System.Collections.Generic;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakerHudScreen : Screen
    {
        private IApplication application;

        private List<Button> buttons;

        Button stateText;

        private MapEditState state;

        public MapMakerHudScreen(IApplication application) : base(application)
        {
            this.application = application;

            this.buttons = new List<Button>();

            application.Window.KeyPressed += OnKeyPress;

            var stateTextPosition = new Vector2f(application.Window.Size.X / 2, application.Window.Size.Y / 2);

            stateText = new Button(
                "Small Test", 
                stateTextPosition,
                () => { },
                HorizontalAlignment.Centre);

            buttons.Add(new Button("Draw", new Vector2f(20, 50), () => state = MapEditState.DrawingLines, HorizontalAlignment.Left));
            buttons.Add(new Button("Move", new Vector2f(20, 100), () => state = MapEditState.MovingPoints, HorizontalAlignment.Left));
            buttons.Add(new Button("Delete", new Vector2f(20, 150), () => state = MapEditState.Deletion, HorizontalAlignment.Left));
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (IsUpdate)
            {

            }
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            buttons.ForEach(b => b.OnUpdate());
        }

        public override void OnRender(RenderTarget target)
        {
            target.SetView(application.GetDefaultView());

            buttons.ForEach(b => b.OnRender(target));

            stateText.OnRender(target);
        }
    }
}
