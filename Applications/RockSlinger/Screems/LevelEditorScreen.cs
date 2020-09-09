using RockSlinger.CrateTools;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Interfaces;

namespace RockSlinger.Screems
{
    public class LevelEditorScreen : GameScreen
    {
        CrateManager crateManager;
        RectangleShape potentialCratePosition;
        Texture potentialCrateTexture;
        Texture crateTexture;
        Texture crateBrokenTexture;
        Texture crateTattersTexture;

        public LevelEditorScreen(
            IApplication application, 
            IApplicationInstance applicationInstance) : base(application, applicationInstance)
        {
            var config = application.Configuration;
            application.Window.MouseButtonPressed += MouseButtonPressed;
            application.Window.MouseButtonPressed += MouseButtonReleased;
            crateManager = new CrateManager(config);
            potentialCrateTexture = new Texture(new Image("Resources\\PotentialCrate.png"));
            crateTexture = new Texture(new Image("Resources\\Crate.png"));
            crateBrokenTexture = new Texture(new Image("Resources\\CrateBroken.png"));
            crateTattersTexture = new Texture(new Image("Resources\\CrateVeryBroken.png"));
        }

        public override void OnUpdate(float deltaT)
        {
            base.OnUpdate(deltaT);

            var mousePosition = GetMousePosition();
            if (!crateManager.IsCrateValid(mousePosition))
            {
                return;
            }

            var crateIndex = crateManager.GetCrateIndexFromPosition(mousePosition);

            var position = crateManager.GetCrateCentreFromIndex(crateIndex);
            potentialCratePosition = new RectangleShape(new Vector2f(64, 64));
            potentialCratePosition.Texture = potentialCrateTexture;
            potentialCratePosition.Origin = new Vector2f(32, 32);
            potentialCratePosition.Position = position;
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);

            if (potentialCratePosition != null)
            {
                target.Draw(potentialCratePosition);
            }

            foreach (var cratePosition in crateManager.GetCrates())
            {
                if (cratePosition == null)
                {
                    continue;
                }

                var crate = new RectangleShape(new Vector2f(64, 64));
                crate.Texture = GetCrateTexture(cratePosition.CrateType);
                crate.Origin = new Vector2f(32, 32);
                crate.Position = cratePosition.Centroid;
                target.Draw(crate);
            }
        }

        private Texture GetCrateTexture(CrateType crateType)
        {
            if (crateType == CrateType.Full)
            {
                return crateTexture;
            }
            if (crateType == CrateType.Broken)
            {
                return crateBrokenTexture;
            }
            if (crateType == CrateType.Tatters)
            {
                return crateTattersTexture;
            }

            return crateTexture;
        }

        private void MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = GetMousePosition();
            crateManager.AddCrate(mousePosition);
        }

        private void MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = GetMousePosition();
        }
    }
}
