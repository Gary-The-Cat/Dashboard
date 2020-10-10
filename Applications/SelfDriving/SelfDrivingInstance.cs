using Ninject;
using SelfDriving.Screens;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using Shared.Interfaces.Services;

namespace SelfDriving
{
    public class SelfDrivingInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private IApplicationService appService;

        public SelfDrivingInstance(IApplicationService appService)
        {
            this.appService = appService;

            Texture texture = new Texture(new Image("Resources\\SelfDriving.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Self Driving";

        public new void Initialize()
        {
            var homeScreen = appService.Kernel.Get<SelfDrivingHomeScreen>();

            AddChildScreen(homeScreen);

            homeScreen.InitializeScreen();

            base.Initialize();
        }
    }
}
