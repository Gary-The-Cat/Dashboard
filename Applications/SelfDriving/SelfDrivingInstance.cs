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

            var track = MapStructure.GetTrack();
            var trackString = JsonConvert.SerializeObject(track, Formatting.Indented);
            File.WriteAllText("C:\\Test\\Track_1.json", trackString);
        }

        public IApplication Application { get; set; }

        public string DisplayName => "Self Driving";

        public RectangleShape Thumbnail { get; set; }

        public override Screen Screen { get; set; }

        public RenderWindow RenderWindow { get; set; }

        public new void Start()
        {
            base.Start();
            this.Screen = new SelfDrivingHomeScreen(Application);
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
