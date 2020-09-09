using Newtonsoft.Json;
using SelfDriving.Screens;
using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using System.IO;

namespace SelfDriving
{
    public class SelfDrivingInstance : ApplicationInstanceBase, IApplicationInstance
    {

        public SelfDrivingInstance(IApplication application)
        {
            this.Application = application;

            Texture texture = new Texture(new Image("Resources\\SelfDriving.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public void AddScreen(Screen screen) => ScreenManager.AddScreen(screen);

        public void RemoveScreen(Screen screen) => ScreenManager.RemoveScreen(screen);

        public IApplication Application { get; set; }

        public string DisplayName => "Self Driving";

        public RectangleShape Thumbnail { get; set; }

        public RenderWindow RenderWindow { get; set; }

        public new void Initialize()
        {
            AddScreen(new SelfDrivingHomeScreen(Application, this));

            base.Initialize();
        }

        public new void Start()
        {
            base.Start();
        }
    }
}
