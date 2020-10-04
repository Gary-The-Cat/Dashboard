using ORToolsDemo.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace ORToolsDemo
{
    public class ORToolsDemoInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public ORToolsDemoInstance(IApplication application) : base(application)
        {
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
            AddChildScreen(new ORToolsDemoScreen(Application, this), null);

            base.Initialize();
        }
    }
}
