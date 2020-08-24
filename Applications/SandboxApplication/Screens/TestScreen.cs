using SFML.Graphics;
using SFML.System;
using Shared.CameraTools;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace SandboxApplication.Screens
{
    public class TestScreen : Screen
    {
        private IApplication application;
        private RectangleShape sprite;
        private RenderStates renderState;
        private Color color;
        private Camera camera;

        public TestScreen(IApplication application, Color color)
        {
            this.application = application;

            this.camera = new Camera(
                application.Configuration.SinglePlayer, 
                application.Configuration);

            var shader = new Shader(
                "Shaders/Demo/shader.vert",
                null,
                "Shaders/Demo/shader.frag");

            this.color = new Color(0x3e, 0x3e, 0xe1);

            this.renderState = new RenderStates(shader);

            var size = application.Window.Size;

            sprite = new RectangleShape
            {
                Position = new Vector2f(size.X / 2, size.Y / 2),
                Origin = new Vector2f(100, 100),
                Size = new Vector2f(200, 200),
                OutlineThickness = 16.0f,
                OutlineColor = color,
                FillColor = color
            };
        }

        public override void OnEnter()
        {
            Console.WriteLine("TestScreen: Enter");
        }

        public override void OnUpdate(float dt)
        {
            sprite.Rotation += 90 * dt;

            camera.Update(dt);
        }

        public override void OnRender(RenderTarget target)
        {
            target.SetView(camera.GetView());
            target.Draw(sprite);
        }

        public override void OnExit()
        {
            Console.WriteLine("TestScreen: Exit");
        }
    }
}
