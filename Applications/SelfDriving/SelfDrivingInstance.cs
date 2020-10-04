using SelfDriving.Screens;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;

namespace SelfDriving
{
    public class SelfDrivingInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public SelfDrivingInstance(IApplication application) : base(application)
        {
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
            var MainScreen = new SelfDrivingHomeScreen(Application, this);

            AddChildScreen(MainScreen, null);

            MainScreen.InitializeScreen();

            base.Initialize();
        }
    }
}
