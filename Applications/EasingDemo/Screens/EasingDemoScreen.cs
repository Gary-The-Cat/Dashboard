using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using Shared.Maths;
using System;
using System.Diagnostics;

namespace EasingDemo.Screens
{
    class EasingDemoScreen : Screen
    {
        private RectangleShape sprite;

        public EasingDemoScreen(IApplication application, Color color)
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

            var worker = new EasingWorker(
                Easings.EaseInOutCirc,
                a => 
                {
                    sprite.Scale = new Vector2f((float)a, (float)a);
                }, 
                2000,
                2);
        }

        public override void OnEnter()
        {
            Debug.WriteLine("EasingDemoScreen: Enter");
        }

        public override void OnUpdate(float dt)
        {

        }

        public override void OnRender(RenderTarget target)
        {
            target.Draw(sprite);
        }

        public override void OnExit()
        {
            Console.WriteLine("EasingDemoScreen: Exit");
        }
    }
}
