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

        public IApplication Application { get; set; }

        public string DisplayName => "Self Driving";

        public RectangleShape Thumbnail { get; set; }

        public override Screen Screen { get; set; }

        public RenderWindow RenderWindow { get; set; }

        public new void Initialize()
        {
            base.Initialize();

            Screen = new SelfDrivingHomeScreen(Application);
            Application.ApplicationManager.AddScreen(this.Screen);
        }

        public new void Start()
        {
            base.Start();
        }
    }
}
