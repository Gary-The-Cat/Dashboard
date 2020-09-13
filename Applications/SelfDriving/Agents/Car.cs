using SelfDriving.DataStructures;
using SelfDriving.Helpers;
using SelfDriving.Interfaces;
using SelfDriving.Managers;
using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using Shared.CollisionData;
using Shared.ExtensionMethods;
using Shared.FireBase;
using System;
using System.Collections.Generic;

namespace SelfDriving.Agents
{
    public class Car
    {
        public Guid Id { get; }

        public ICarAI Controller { get; private set; }

        public CarConfiguration Configuration { get; }

        // Car physical attributes
        public Vector2f Position;

        public float Heading { get; private set; }

        private float initialHeading;

        private float speed;

        // Progress tracking
        private CheckpointManager checkpointManager;
        public bool IsRunning { get; private set; }

        // When running, swapping logic in/out for fitness/culling is beneficial.
        private List<Func<Car, float>> fitnessMetrics;
        private List<Func<Car, bool>> cullMetrics;

        // Collision
        private List<LineSegment> map;
        private RectangleShape body;

        // Controller fitness tracking
        public float TotalDistance { get; set; }

        public float TimeAlive { get; set; }

        public int CheckpointsPassed => checkpointManager.CheckpointsPassed;

        public Vector2f?[] Collisions { get; private set; }

        public List<LineSegment> Raycasts { get; private set; }

        public Car(ICarAI controller)
        {
            this.Id = Guid.NewGuid();

            // Set the configuration
            this.Configuration = controller.Configuration;

            // Set the controller for the car
            this.Controller = controller;

            this.fitnessMetrics = new List<Func<Car, float>>();
            this.cullMetrics = new List<Func<Car, bool>>();

            // Create the containers to hold the raycast visuals that will be updated each frame
            Raycasts = new List<LineSegment>();
            for (int i = 0; i < this.Configuration.NumberOfRays; i++)
            {
                Raycasts.Add(new LineSegment(0, 0, 0, 0));
            }

            this.checkpointManager = new CheckpointManager();

            this.body = new RectangleShape(this.Configuration.CarSize)
            {
                Origin = this.Configuration.CarSize / 2,
            };
        }

        public void OnUpdate(float deltaT)
        {
            if (!IsRunning)
            {
                return;
            }

            // Update our raycasts to account for the movement in the last frame
            Raycasts = GetRaycasts();
            var (rayDistances, collisions) = CollisionHelper.GetRaycastCollisions(Raycasts, map);

            this.Collisions = collisions;

            // Get the next action to take
            var output = Controller.GetOutput(rayDistances, Position, Heading, checkpointManager.CurrentWaypoint);

            // Update the speed
            speed += output.Acceleration * Configuration.AccelerationCoefficient * deltaT;
            speed -= speed * Configuration.DragCoefficient;

            // Apply the max speed
            speed = Math.Min(speed, Configuration.MaximumForwardSpeed);
            speed = Math.Max(speed, Configuration.MaximumReverseSpeed);

            // Apply the desired rotation
            var turningForce = -output.LeftTurnForce + output.RightTurnForce;

            // Is this correct? 'Maximum' makes it sound like it should be capped.
            Heading += Configuration.MaximumRotationRate * deltaT * turningForce;

            // Update position
            var previousPosition = new Vector2f(Position.X, Position.Y);
            Position.X += (float)Math.Cos(Heading + 1.5708) * speed * deltaT;
            Position.Y += (float)Math.Sin(Heading + 1.5708) * speed * deltaT;

            checkpointManager.Update(Position);

            body.Position = Position;
            body.Rotation = Heading;

            IsRunning = !CheckMapCollision();

            // Kill any cars that have stopped moving, their state will never change,
            // and they will be stuck in this position forever.
            if (TimeAlive > 5)
            {
                ////IsRunning &= speed > 0;
            }

            // If we are not running, skip the fitness update.
            if (IsRunning)
            {
                this.UpdateFitness(deltaT, previousPosition, Position);
            }
            else
            {
                Controller.KillCar();
            }

            this.TotalDistance += previousPosition.Magnitude(Position);
        }

        private bool CheckMapCollision()
        {
            foreach (var segment in map)
            {
                if (CheckCollision(segment.Start, segment.End))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckCollision(Vector2f a, Vector2f b)
        {
            var p1 = body.Transform.TransformPoint(body.GetPoint(0));
            var p2 = body.Transform.TransformPoint(body.GetPoint(1));
            var p3 = body.Transform.TransformPoint(body.GetPoint(2));
            var p4 = body.Transform.TransformPoint(body.GetPoint(3));

            return
                CollisionManager.CheckCollision(a, b, p1, p2) != null ||
                CollisionManager.CheckCollision(a, b, p2, p3) != null ||
                CollisionManager.CheckCollision(a, b, p3, p4) != null ||
                CollisionManager.CheckCollision(a, b, p4, p1) != null;
        }

        public void ResetCar()
        {
            // Set the car back to its running state
            IsRunning = true;

            // Clear the speed and heading so that we are facing the right direction and not moving
            speed = 0;

            Heading = initialHeading;

            Controller.Reset();
        }

        public void AddFitnessMetric(Func<Car, float> fitnessMetric)
        {
            this.fitnessMetrics.Add(fitnessMetric);
        }

        public void AddCullMetric(Func<Car, bool> cullMetric)
        {
            this.cullMetrics.Add(cullMetric);
        }

        public void SetCarStartState(Track track)
        {
            // Update our map! Used for collision detection
            this.map = track.Map;

            // Set the new starting position, and update the position to match
            this.Position = track.StartPosition;

            // Set the initial heading, tracks may start with us facing any direction.
            this.initialHeading = track.InitialHeading;

            // Update the initial heading
            this.Heading = track.InitialHeading;

            // Initialize the checkpoint manager for the new course & set current waypoint
            // :TODO: Update the checkpoint manager to work with line segments
            ////this.checkpointManager.Initialize(track.Checkpoints);

            // Initialize the controller (AI)
            this.Controller.Initalize(Configuration);
        }

        private List<LineSegment> GetRaycasts()
        {
            var casts = new List<LineSegment>();

            // Get left most point (anti-clockwise)
            var start = (Heading / Math.PI * 180) - (Configuration.FieldOfView / 2) + 90;
            var step = Configuration.FieldOfView / (Configuration.NumberOfRays - 1);

            // Loop over each of the rays and update their positions
            for (int i = 0; i < Configuration.NumberOfRays; i++)
            {
                var angle = start + step * i;
                var angleInRadians = angle / 180 * Math.PI;

                var rayStart = new Vector2f(
                    Position.X + 10 * (float)Math.Cos(angleInRadians),
                    Position.Y + 10 * (float)Math.Sin(angleInRadians));

                var rayEnd = new Vector2f(
                    Position.X + Configuration.RayLength * (float)Math.Cos(angleInRadians),
                    Position.Y + Configuration.RayLength * (float)Math.Sin(angleInRadians));

                casts.Add(new LineSegment(rayStart, rayEnd));
            }

            return casts;
        }

        private void UpdateFitness(float deltaT, Vector2f previousPosition, Vector2f currentPosition)
        {
            // Get the distance we have covered in the last frame
            var distance = currentPosition.Magnitude(previousPosition);

            // Increment our total distance and alive time
            TotalDistance += distance;
            TimeAlive += deltaT;

            var fitness = 0f;

            fitnessMetrics.ForEach(m => fitness += m(this));

            // Pass the updated fitness values to our controller
            Controller.SetFitness(fitness);
        }
    }
}
