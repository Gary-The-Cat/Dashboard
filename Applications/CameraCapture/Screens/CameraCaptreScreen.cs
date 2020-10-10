using OpenCvSharp;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using System.IO;

namespace CameraCapture.Screens
{
    public class CameraCaptreScreen : Screen
    {
        RectangleShape sprite;

        string tempImagePath;

        VideoCapture capture;

        public CameraCaptreScreen(IApplicationManager appManager)
        {
            var windowSize = appManager.GetWindowSize();
            var size = new Vector2f(windowSize.X, windowSize.Y);

            capture = new VideoCapture(0);

            sprite = new RectangleShape
            {
                Position = size / 2,
                Origin = size / 2,
                Size = size,
                FillColor = Color.Blue
            };

            tempImagePath = $"{Path.GetTempFileName()}.png";
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (capture.Grab())
            {
                var image = Mat.Zeros(capture.FrameHeight, capture.FrameWidth, MatType.CV_8UC3);
                var output = OutputArray.Create(image);
                capture.Read(output);

                Cv2.ImWrite(tempImagePath, image);
            }
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);

            var texture = new Texture(tempImagePath);

            sprite.Size = new Vector2f(texture.Size.X / 2, texture.Size.Y / 2);
            sprite.Origin = new Vector2f(texture.Size.X / 4, texture.Size.Y / 4);
            sprite.Texture = texture;
            target.Draw(sprite);
        }
    }
}
