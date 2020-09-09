using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Interfaces;
using Shared.ScreenConfig;
using System.Collections.Generic;

namespace RockSlinger.Screems
{
    public class GameScreen : Screen
    {
        readonly Vector2f TitleBarSize = new Vector2f(12, 57);

        List<Drawable> entities;

        public CircleShape CreateCircleShape(Vector2f pos, int radius) => new CircleShape(radius) { Position = pos, Origin = new Vector2f(radius, radius) };

        public Vertex[] dirt;
        public Vertex[] grass;
        public Vertex[] background;
        private ScreenConfiguration Configuration { get; set; }

        private IApplication Application { get; set; }

        public GameScreen(
            IApplication application,
            IApplicationInstance applicationInstance) 
            : base(application.Configuration, applicationInstance)
        {
            this.Application = application;

            this.Configuration = application.Configuration;

            entities = new List<Drawable>();

            var dirtBottom = new Color(0x62, 0x43, 0x32);
            var dirtTop = new Color(0x9B, 0x6B, 0x4F);

            var grassBottom = new Color(0x4C, 0x91, 0x4B);
            var grassTop = new Color(0x4C, 0x91, 0x4B);

            var backgroundBottom = new Color(0x87, 0xB5, 0xC2);
            var backgroundTop = new Color(0x4B, 0x7D, 0x91);

            var dirtHeight = 76;
            var grassHeight = 8;
            var topOfGrass = dirtHeight + grassHeight;

            dirt = new Vertex[]
            {
                new Vertex(new Vector2f(0, Configuration.Height - dirtHeight), dirtBottom),
                new Vertex(new Vector2f(Configuration.Width, Configuration.Height - dirtHeight), dirtBottom),
                new Vertex(new Vector2f(Configuration.Width, Configuration.Height), dirtTop),
                new Vertex(new Vector2f(0, Configuration.Height), dirtTop),
            };

            grass = new Vertex[]
            {
                new Vertex(new Vector2f(0, Configuration.Height - topOfGrass), grassBottom),
                new Vertex(new Vector2f(Configuration.Width, Configuration.Height - topOfGrass), grassBottom),
                new Vertex(new Vector2f(Configuration.Width, Configuration.Height - dirtHeight), grassTop),
                new Vertex(new Vector2f(0, Configuration.Height - dirtHeight), grassTop),
            };

            background = new Vertex[]
            {
                new Vertex(new Vector2f(0, Configuration.Height), backgroundTop),
                new Vertex(new Vector2f(Configuration.Width, Configuration.Height), backgroundTop),
                new Vertex(new Vector2f(Configuration.Width, 0), backgroundTop),
                new Vertex(new Vector2f(0, 0), backgroundBottom),
            };
        }

        /// <summary>
        /// Update - Here we add all our logic for updating components in this screen. 
        /// This includes checking for user input, updating the position of moving objects and more!
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void OnUpdate(float deltaT)
        {

        }

        /// <summary>
        /// Draw - Here we don't update any of the components, only draw them in their current state to the screen.
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void OnRender(RenderTarget target)
        {
            target.Draw(background, 0, 4, PrimitiveType.Quads);
            target.Draw(dirt, 0, 4, PrimitiveType.Quads);
            target.Draw(grass, 0, 4, PrimitiveType.Quads);

            foreach (var entity in entities)
            {
                target.Draw(entity);
            }
        }

        public Vector2f GetMousePosition()
        {
            var position = Mouse.GetPosition();

            var adjustedPosition = position - Application.Window.Position;

            return new Vector2f(adjustedPosition.X - TitleBarSize.X, adjustedPosition.Y - TitleBarSize.Y);
        }

        public void AddVisual(Drawable visual)
        {
            entities.Add(visual);
        }
    }
}
