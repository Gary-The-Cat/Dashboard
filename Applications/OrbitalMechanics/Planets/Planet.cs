using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrbitalMechanics.Planets
{
    public class Planet
    {
        public Planet(Texture texture, Vector2f position, float radius, float mass)
        {
            this.Texture = texture;
            this.Position = position;
            this.Radius = radius;
            this.Mass = mass;

            this.PlanetBody = new CircleShape(radius)
            {
                Texture = texture,
                Position = this.Position,
                OutlineColor = new Color(255, 255, 255, 164),
                OutlineThickness = 3,
                Origin = new Vector2f(radius, radius),
            };
        }

        public Vector2f Position { get; set; }

        public float Radius { get; set; }

        public Texture Texture { get; set; }

        public float Mass { get; set; }

        public CircleShape PlanetBody { get; set; }
    }
}
