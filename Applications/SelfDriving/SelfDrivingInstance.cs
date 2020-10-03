using Newtonsoft.Json;
using SelfDriving.Screens;
using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Core.Hierarchy;
using Shared.Interfaces;
using System.IO;

namespace SelfDriving
{
    public class SelfDrivingInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public SelfDrivingInstance(IApplication application) : base(application)
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

        public Screen MainScreen { get; set; }

        public new void Initialize()
        {
            MainScreen = new SelfDrivingHomeScreen(Application, this);

            AddChildScreen(MainScreen, null);

            MainScreen.InitializeScreen();

            SetActiveScreen(MainScreen);

            base.Initialize();
        }

        public new void Start()
        {
            MainScreen.SetActive();

            base.Start();
        }
    }
}
