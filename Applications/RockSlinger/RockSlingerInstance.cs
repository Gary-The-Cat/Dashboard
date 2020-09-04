using RockSlinger.Screems;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace RockSlinger
{
    public class RockSlingerInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public RockSlingerInstance(IApplication application)
        {
            this.Application = application;

            Texture texture = new Texture(new Image("Resources\\RockSlinger.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public IApplication Application { get; set; }

        public string DisplayName => "Rock Slinger";

        public RectangleShape Thumbnail { get; set; }

        public override Screen Screen { get; set; }

        public RenderWindow RenderWindow { get; set; }

        public new void Initialize()
        {
            base.Initialize();

            Screen = new LevelEditorScreen(Application);

            Application.ApplicationManager.AddScreen(Screen);
        }

        public new void Start()
        {
            base.Start();

            Application.Window.SetMouseCursorVisible(true);
        }
    }
}
