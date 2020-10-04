using OrbitalMechanics.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;

namespace OrbitalMechanics
{
    public class OrbitalMechanicsInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public OrbitalMechanicsInstance(IApplication application) : base(application)
        {
            Texture texture = new Texture(new Image("Resources\\OrbitalMechanics.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Orbital Mechanics";

        public new void Initialize()
        {
            AddChildScreen(new OrbitalMechanicsScreen(Application, this), null);

            base.Initialize();
        }
    }
}
