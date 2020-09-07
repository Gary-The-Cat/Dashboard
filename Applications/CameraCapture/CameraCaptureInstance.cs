using CameraCapture.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace CameraCapture
{
    public class CameraCaptureInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public CameraCaptureInstance(IApplication application)
        {
            this.Application = application;

            Texture texture = new Texture(new Image("Resources\\CameraCapture.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public IApplication Application { get; set; }

        public string DisplayName => "Camera Capture";

        public RectangleShape Thumbnail { get; set; }

        public override Screen Screen { get; set; }

        public RenderWindow RenderWindow { get; set; }

        public new void Initialize()
        {
            Screen = new CameraCaptreScreen(Application);
            Application.ApplicationManager.AddScreen(Screen);

            base.Initialize();
        }

        public new void Start()
        {
            base.Start();
        }

        public void OnUpdate(float deltaT)
        {
            Screen.OnUpdate(deltaT);
        }

        public void OnRender(RenderTarget target)
        {
            Screen.OnRender(target);
        }
    }
}
