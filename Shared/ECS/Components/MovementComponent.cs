using SFML.System;

namespace Shared.ECS.Components
{
    public class MovementComponent
    {
        public MovementComponent()
        {
            Velocity = new Vector2f(0, 0);
        }

        public Vector2f Velocity;

        public float RotationalVelocity;
    }
}
