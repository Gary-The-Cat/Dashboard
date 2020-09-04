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

        private MapMakerDataContainer sharedContainer;
        Button stateText;

        public MapMakerHudScreen(
            IApplication application, 
            MapMakerDataContainer sharedContainer) : base(application.Configuration)
        {
            this.application = application;

            this.sharedContainer = sharedContainer;

            this.buttons = new List<Button>();

            application.Window.KeyPressed += OnKeyPress;

            application.Window.MouseButtonPressed += OnMousePress;

            var stateTextPosition = new Vector2f(application.Window.Size.X / 2, application.Window.Size.Y / 2);

            stateText = new Button(
                "Small Test", 
                stateTextPosition,
                () => { },
                HorizontalAlignment.Centre);

            buttons.Add(new Button("Draw", new Vector2f(20, 20), () => SetState(MapEditState.DrawingLines), HorizontalAlignment.Left));
            buttons.Add(new Button("Move", new Vector2f(20, 70), () => SetState(MapEditState.MovingPoints), HorizontalAlignment.Left));
            buttons.Add(new Button("Delete", new Vector2f(20, 120), () => SetState(MapEditState.Deletion), HorizontalAlignment.Left));
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

        private void SetState(MapEditState state)
        {
            this.sharedContainer.EditState = state;
            this.stateText.Text = state.ToString();
        }

        private void OnMousePress(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsUpdate)
            {
                return;
            }

            var position = new Vector2f(e.X, e.Y);

            buttons.ForEach(b => 
            {
                if (b.GetGlobalBounds().Contains(e.X, e.Y))
                {
                    b.OnClick();
                }
            });
        }
    }
}
