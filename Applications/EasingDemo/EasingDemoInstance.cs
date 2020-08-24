using EasingDemo.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace SandboxApplication.DemoApplication
{
    public class EasingDemoInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public EasingDemoInstance(IApplication application)
        {
            this.Application = application;

            Texture texture = new Texture(new Image("Resources\\EasingIcon.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public IApplication Application { get; set; }

        public string DisplayName => "Easing Demo";

        public RectangleShape Thumbnail { get; set; }

        public override Screen Screen { get; set; }

        public RenderWindow RenderWindow { get; set; }

        public new void Start()
        {
            base.Start();
            this.Screen = new EasingDemoScreen(Application, Color.Blue);
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
