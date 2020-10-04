using RockSlinger.Screems;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace RockSlinger
{
    public class RockSlingerInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public RockSlingerInstance(IApplication application) : base(application)
        {
            Texture texture = new Texture(new Image("Resources\\RockSlinger.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Rock Slinger";

        public new void Initialize()
        {
            AddChildScreen(new LevelEditorScreen(Application, this), null);

            base.Initialize();
        }
    }
}
