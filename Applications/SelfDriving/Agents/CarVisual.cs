using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SelfDriving.Agents
{
    public class CarVisual
    {
        internal Vector2f Position => car.Position;

        private Car car;

        private RectangleShape sprite;

        private List<Vertex[]> raycasts;

        public float TotalDistance => car.TotalDistance;

        public bool IsRunning => car.IsRunning;

        public CarVisual(Car car)
        {
            this.car = car;

            // Set the sprite for the cars visual, origin at centre makes turning easier to handle (logically).
            sprite = new RectangleShape(car.Configuration.CarSize)
            {
                OutlineThickness = 2,
                OutlineColor = Color.White,
                Origin = car.Configuration.CarSize / 2
            };

            raycasts = new List<Vertex[]>();
            for (int i = 0; i < car.Configuration.NumberOfRays; i++)
            {
                var ray = new Vertex[2];
                ray[0] = new Vertex() { Color = Color.Black };
                ray[1] = new Vertex() { Color = Color.Black };

                raycasts.Add(ray);
            }
        }

        public void OnUpdate(float deltaT)
        {
            // Update the sprite to match the state of our car
            sprite.Position = car.Position;
            sprite.Rotation = car.Heading * 180 / (3.14159265358f);

            for (int i = 0; i < car.Configuration.NumberOfRays; i++)
            {
                var ray = raycasts[i];
                var collision = car.Collisions[i];

                if (collision.HasValue)
                {
                    ray[0].Position = car.Raycasts[i].Start;
                    ray[1].Position = collision.Value;
                    ray[1].Color = Color.Red;
                }
                else
                {
                    ray[0].Position = car.Raycasts[i].Start;
                    ray[1].Position = car.Raycasts[i].End;
                    ray[1].Color = Color.Blue;
                }
            }
        }

        public void OnRender(RenderTarget target)
        {
            if (!IsRunning)
            {
                // If we are not running, our outline is dark
                sprite.OutlineColor = new Color(0x1e, 0x1e, 0x1e);
            }
            else
            {
                // If we are running, show the raycasts
                raycasts.ForEach(r => target.Draw(r, 0, 2, PrimitiveType.Lines));
            }

            target.Draw(sprite);
        }

        public void ResetCar()
        {
            // Car is operational, set outline to white to show it's active
            sprite.OutlineColor = Color.White;
        }
    }
}
