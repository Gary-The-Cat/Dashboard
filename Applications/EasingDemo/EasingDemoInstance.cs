using EasingDemo.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace SandboxApplication.DemoApplication
{
    public class EasingDemoInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public EasingDemoInstance(IApplication application) : base(application)
        {
            Texture texture = new Texture(new Image("Resources\\EasingIcon.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Easing Demo";

        public new void Initialize()
        {
            AddChildScreen(new EasingDemoScreen(Application, this, Color.Blue), null);

            base.Initialize();
        }
    }
}
