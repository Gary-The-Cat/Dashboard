using SFML.System;

namespace SelfDriving.DataStructures
{
    public class CarConfiguration
    {
        public int AccelerationCoefficient => 360;

        public int BrakeCoefficient => 70;

        public Vector2f CarSize => new Vector2f(20, 45);

        // Field of view is in degrees
        public float FieldOfView => 270;

        // Ray lengths are measured in pixels.
        public float RayLength => 1000;

        public int NumberOfRays { get; set; } = 21;

        public int MaximumForwardSpeed => 1000;

        public int MaximumReverseSpeed => -400;

        public int MaximumRotationRate => 2;

        public float DragCoefficient => 0.015f;
    }
}
