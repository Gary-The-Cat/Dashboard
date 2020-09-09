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

        public RenderWindow RenderWindow { get; set; }

        public void AddScreen(Screen screen) => ScreenManager.AddScreen(screen);

        public void RemoveScreen(Screen screen) => ScreenManager.RemoveScreen(screen);

        public new void Initialize()
        {
            AddScreen(new LevelEditorScreen(Application, this));

            base.Initialize();
        }

        public new void Start()
        {
            base.Start();

            Application.Window.SetMouseCursorVisible(true);
        }
    }
}
