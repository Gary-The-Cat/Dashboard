using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.ScreenConfig;

namespace Shared.CameraTools
{
    public class Camera
    {
        public Vector2f Position { get; set; }

        public FloatRect ViewPort;

        public float Rotation { get; set; }

        public float Zoom { get; set; } = 1;

        private View view;

        private ScreenConfiguration configuration;

        public Camera(ScreenConfiguration configuration)
        {
            Position = new Vector2f(0, 0);

            this.view = new View(new FloatRect(
                new Vector2f(0, 0),
                new Vector2f(configuration.Width, configuration.Height)));

            this.ViewPort = configuration.SinglePlayer;
            this.configuration = configuration;
        }

        public void Update(float deltaT)
        {
            if (!configuration.AllowCameraMovement)
            {
                return;
            }

            var isControlPressed =
                Keyboard.IsKeyPressed(Keyboard.Key.LControl) ||
                Keyboard.IsKeyPressed(Keyboard.Key.RControl);

            var ratio = configuration.Width / (float)configuration.Height;
            var offset = new Vector2f(0, 0);
            if (Keyboard.IsKeyPressed(configuration.PanUp) && !isControlPressed)
            {
                offset.Y -= 400f * deltaT;
            }

            if (Keyboard.IsKeyPressed(configuration.PanLeft) && !isControlPressed)
            {
                offset.X -= 400f * deltaT / ratio;
            }

            if (Keyboard.IsKeyPressed(configuration.PanDown) && !isControlPressed)
            {
                offset.Y += 400f * deltaT;
            }

            if (Keyboard.IsKeyPressed(configuration.PanRight) && !isControlPressed)
            {
                offset.X += 400f * deltaT / ratio;
            }

            if (Keyboard.IsKeyPressed(configuration.ZoomIn) && !isControlPressed)
            {
                this.Zoom += 0.1f * deltaT;
            }
            else if (Keyboard.IsKeyPressed(configuration.ZoomOut) && !isControlPressed)
            {
                this.Zoom -= 0.1f * deltaT;
            }
            else
            {
                this.Zoom = 1;
            }

            if (Keyboard.IsKeyPressed(configuration.RotateLeft))
            {
                this.Rotation -= 45f * deltaT;
            }
            if (Keyboard.IsKeyPressed(configuration.RotateRight))
            {
                this.Rotation += 45f * deltaT;
            }

            this.Position += offset;

            this.view.Rotation = this.Rotation;

            this.view.Viewport = this.ViewPort;

            this.view.Move(offset);

            this.view.Zoom(this.Zoom);
        }

        public void SetCentre(Vector2f centre, float proportion = 1)
        {
            var difference = (this.view.Center - centre) * proportion;

            this.view.Center = this.view.Center -= difference;
        }

        public void ScaleToWindow(float width, float height)
        {
            var viewAspect = GetAspectRatio();
            var windowAspect = width / height;

            this.ViewPort = new FloatRect(0, 0, 1, 1);
            if (windowAspect > viewAspect)
            {
                this.ViewPort.Width = viewAspect / windowAspect;
                this.ViewPort.Left = (1f - this.ViewPort.Width) / 2f;
            }
            else
            {
                this.ViewPort.Height = windowAspect / viewAspect;
                this.ViewPort.Top = (1 - this.ViewPort.Height) / 2f;
            }
        }

        private float GetAspectRatio()
        {
            return this.view.Viewport.Width / this.view.Viewport.Height;
        }

        public View GetView()
        {
            return view;
        }
    }
}
