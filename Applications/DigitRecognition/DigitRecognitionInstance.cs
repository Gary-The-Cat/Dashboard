using DigitRecognition.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace DigitRecognition
{
    public class DigitRecognitionInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public DigitRecognitionInstance(IApplication application) : base(application)
        {
            Texture texture = new Texture(new Image("Resources\\DigitRecognition.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Digit Recognition";

        public new void Initialize()
        {
            AddChildScreen(new DigitRecognitionScreen(Application, this), null);

            base.Initialize();
        }
    }
}
