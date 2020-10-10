using DigitRecognition.Screens;
using Ninject;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using System;

namespace DigitRecognition
{
    public class DigitRecognitionInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private IApplicationService appService;

        public DigitRecognitionInstance(IApplicationService appService)
        {
            Texture texture = new Texture(new Image("Resources\\DigitRecognition.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            this.appService = appService;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Digit Recognition";

        public new void Initialize()
        {
            var digitRecognition = appService.Kernel.Get<DigitRecognitionScreen>();

            AddChildScreen(digitRecognition);

            base.Initialize();
        }
    }
}
