using CameraCapture.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace CameraCapture
{
    public class CameraCaptureInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public CameraCaptureInstance(IApplication application) : base(application)
        {
            Texture texture = new Texture(new Image("Resources\\CameraCapture.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Camera Capture";

        public new void Initialize()
        {
            AddChildScreen(new CameraCaptreScreen(Application, this), null);

            base.Initialize();
        }

        public new void Start()
        {
            base.Start();
        }
    }
}
