using ORToolsDemo.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace ORToolsDemo
{
    public class ORToolsDemoInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public ORToolsDemoInstance(IApplication application)
        {
            this.Application = application;

            Texture texture = new Texture(new Image("Resources\\ORTools.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public IApplication Application { get; set; }

        public string DisplayName => "OR.Tools Demo";

        public RectangleShape Thumbnail { get; set; }

        public void AddScreen(Screen screen) => ScreenManager.AddScreen(screen);

        public void RemoveScreen(Screen screen) => ScreenManager.RemoveScreen(screen);

        public new void Initialize()
        {
            AddScreen(new ORToolsDemoScreen(Application, this));

            base.Initialize();
        }

        public new void Start()
        {
            base.Start();
        }
    }
}
