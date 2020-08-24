using SandboxApplication.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;

namespace SandboxApplication
{
    public class SandboxApplicationInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public SandboxApplicationInstance(IApplication application)
        {
            this.Application = application;

            Texture texture = new Texture(new Image("Resources\\Sandbox.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public IApplication Application { get; set; }

        public string DisplayName => "Sandbox";

        public RectangleShape Thumbnail { get; set; }

        public bool IsInitialized { get; set; }

        public override Screen Screen { get; set; }

        public RenderWindow RenderWindow { get; set; }

        public new void Start()
        {
            base.Start();
            this.Screen = new TestScreen(Application, Color.Red);
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
