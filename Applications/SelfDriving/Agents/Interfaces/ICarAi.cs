using SelfDriving.DataStructures;
using SFML.System;

namespace SelfDriving.Interfaces
{
    public interface ICarAI
    {
        void Initalize(CarConfiguration configuration);

        DrivingAction GetOutput(
            float[] rayCollisions,
            Vector2f carPosition,
            float carHeading,
            Vector2f nextCheckpointPosition);

        double Fitness { get; }

        void SetFitness(float fitness);

        void KillCar();

        void OnUpdate(float deltaT);

        void Reset();

        CarConfiguration Configuration { get; set; }
    }
}
