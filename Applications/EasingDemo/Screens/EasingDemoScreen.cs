using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;

namespace EasingDemo.Screens
{
    class EasingDemoScreen : Screen
    {
        private RectangleShape sprite;

        public EasingDemoScreen(
            IApplication application, 
            IApplicationInstance applicationInstance,
            Color color) : base(application, applicationInstance)
        {
            var size = application.Window.Size;

            sprite = new RectangleShape
            {
                Position = new Vector2f(size.X / 2, size.Y / 2),
                Origin = new Vector2f(100, 100),
                Size = new Vector2f(200, 200),
                Scale = new Vector2f(0,0),
                FillColor = color
            };
        }

        public override void OnRender(RenderTarget target)
        {
            target.Draw(sprite);
        }
    }
}
