using System;
using System.Collections.Generic;
using System.Text;

namespace SelfDriving.Agents
{
    public static class FitnessMetrics
    {
        public static float DistanceMetric(Car car)
        {
            return car.TotalDistance / 250;
        }

        public static float TimeAliveMetric(Car car)
        {
            return car.TimeAlive;
        }

        public static float CheckpointsPassedMetric(Car car)
        {
            return car.CheckpointsPassed * 5;
        }
    }
}
