using SFML.Graphics;
using SFML.System;
using Shared.CollisionData.CollisionShapes;

namespace OrbitalMechanics.Planets
{
    public class Goal
    {
        public Vector2f Position { get; set; }

        public float Radius { get; set; }

        public Circle Body { get; set; }

        public CircleShape GoalCentre { get; set; }

        public CircleShape GoalOuter { get; set; }

        private bool incrementing;

        public Goal(Vector2f position, float radius)
        {
            this.Position = position;
            this.Radius = radius;
            this.Body = new Circle(position.X, position.Y, radius);

            GoalCentre = new CircleShape(radius / 3)
            {
                Position = position,
                FillColor = new Color(0x34, 0xa8, 0x53),
                Origin = new Vector2f(radius / 3, radius / 3)
            };

            GoalOuter = new CircleShape(radius)
            {
                Position = position,
                OutlineColor = Color.White,
                OutlineThickness = 2,
                FillColor = Color.Transparent,
                Origin = new Vector2f(radius, radius)
            };
        }

        public void Draw(RenderTarget target)
        {
            target.Draw(GoalOuter);
            target.Draw(GoalCentre);
        }

        public void Update(float deltaT)
        {
            var alpha = (byte)(255 - (255 * ((int)GoalOuter.Radius) / 30));
            if (incrementing)
            {
                GoalOuter.Radius += 25 * deltaT;
                GoalOuter.Origin = new Vector2f(GoalOuter.Radius, GoalOuter.Radius);
                if (GoalOuter.Radius > 30)
                {
                    incrementing = false;
                }

                GoalOuter.OutlineColor = new Color(GoalOuter.OutlineColor.R, GoalOuter.OutlineColor.G, GoalOuter.OutlineColor.B, alpha);
            }
            else
            {
                GoalOuter.Radius -= 25 * deltaT;
                GoalOuter.Origin = new Vector2f(GoalOuter.Radius, GoalOuter.Radius);
                if (GoalOuter.Radius < 10)
                {
                    incrementing = true;
                }

                GoalOuter.OutlineColor = new Color(GoalOuter.OutlineColor.R, GoalOuter.OutlineColor.G, GoalOuter.OutlineColor.B, 0);
            }
        }
    }
}
