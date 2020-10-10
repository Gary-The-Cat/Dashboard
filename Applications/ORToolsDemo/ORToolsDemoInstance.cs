using Ninject;
using ORToolsDemo.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using Shared.Interfaces.Services;

namespace ORToolsDemo
{
    public class ORToolsDemoInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private IApplicationService appService;

        public ORToolsDemoInstance(IApplicationService appService)
        {
            this.appService = appService;

            Texture texture = new Texture(new Image("Resources\\ORTools.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "OR.Tools Demo";

        public new void Initialize()
        {
            var orToolsDemoScreen = appService.Kernel.Get<ORToolsDemoScreen>();
            AddChildScreen(orToolsDemoScreen);

            base.Initialize();
        }
    }
}
