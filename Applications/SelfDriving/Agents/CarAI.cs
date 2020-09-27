using SelfDriving.DataStructures;
using SelfDriving.Interfaces;
using SFML.System;
using Shared.GeneticAlgorithms;
using Shared.NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SelfDriving.Agents
{
    public class CarAI : ICarAI, IIndividual
    {
        private int InputNodes => this.networkStructure[0];

        public CarAI(Random random, MLPNeuralNetwork network)
        {
            this.networkStructure = network.GetStructure();

            this.Network = network;

            this.Random = random;

            this.Configuration = new CarConfiguration();
        }

        public CarAI(int[] networkStructure, Random random, List<float> initialWeights = null)
        {
            this.networkStructure = networkStructure;

            this.Network = new MLPNeuralNetwork(networkStructure);

            if (initialWeights != null)
            {
                this.Network.UpdateNetworkWeights(initialWeights);
            }

            this.Random = random;

            this.Configuration = new CarConfiguration();
        }

        public double Fitness { get; private set; } = -1;

        public double GetFitness() => Fitness;

        public MLPNeuralNetwork Network { get; set; }

        public Random Random { get; set; }

        private int[] networkStructure { get; }

        public CarConfiguration Configuration { get; set; }

        private Vector2f previousCheckpoint;

        private Vector2f previousCarPosition; 

        public DrivingAction GetOutput(
            float[] rayCollisions,
            Vector2f carPosition,
            float carHeading,
            Vector2f nextCheckpointPosition)
        {
            var networkInput = new float[InputNodes];
            for (int i = 0; i < InputNodes; i++)
            {
                networkInput[i] = rayCollisions[i];
            }

            var networkOutput = Network.FeedForward(networkInput).Select(f => (f + 1) / 2).ToArray();

            var acceleration = Math.Min(networkOutput[0], 1);
            var braking = Math.Min(networkOutput[3], 1);
            var leftTurningForce = Math.Min(networkOutput[1], 1);
            var rightTurningForce = Math.Min(networkOutput[2], 1);

            return new DrivingAction
            {
                Acceleration = acceleration,
                LeftTurnForce = leftTurningForce,
                RightTurnForce = rightTurningForce,
                BreakingForce = braking
            };
        }

        public IIndividual Clone()
        {
            var carAi = new CarAI(this.networkStructure, this.Random, this.Network.GetFlattenedWeights().ToList());
            carAi.Initalize(Configuration);
            return carAi;
        }

        public void Mutate()
        {
            // Get the current weights
            var networkWeights = Network.GetFlattenedWeights();

            for (int i = 0; i < networkWeights.Length; i++)
            {
                if (Random.NextDouble() < 0.01)
                {
                    // Randomly select which one we are going to mutate.
                    var index = Random.Next(0, networkWeights.Length);

                    // Perform the mutation (this should probably include some annealing)
                    networkWeights[index] += (float)(Random.NextDouble() * 0.2 - 0.1);
                }
            }

            // Update the network weights to reflect the mutation.
            Network.UpdateNetworkWeights(networkWeights.ToList());
        }

        public void SetFitness(float fitness)
        {
            Fitness = fitness;
        }

        public void Initalize(CarConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void KillCar()
        {
            // nothing for now
        }

        public void Reset()
        {
            // Nothing for now
        }

        public void OnUpdate(float deltaT)
        {
            // Nothing for now
        }
    }
}
