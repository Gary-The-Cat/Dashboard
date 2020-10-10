using CameraCapture.Screens;
using Ninject;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using Shared.Interfaces.Services;

namespace CameraCapture
{
    public class CameraCaptureInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private IApplicationService appService;

        public CameraCaptureInstance(IApplicationService appService)
        {
            this.appService = appService;

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
            var cameraCaptureScreen = appService.Kernel.Get<CameraCaptreScreen>();
            AddChildScreen(cameraCaptureScreen);

            base.Initialize();
        }

        public new void Start()
        {
            base.Start();
        }
    }
}
