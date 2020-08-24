using SelfDriving.Agents;
using SelfDriving.DataStructures;
using Shared.GeneticAlgorithms;
using System;
using System.Collections.Generic;

namespace SelfDriving.Helpers
{
    public static class GAHelpers
    {
        public static IIndividual SpawnCarController(
            CarConfiguration carConfiguration, 
            Random random, 
            List<float> initialWeights = null)
        {
            var networkStructure = new int[] { carConfiguration.NumberOfRays, 12, 12, 4 };
            var controller = new CarAI(networkStructure, random, initialWeights);
            controller.Initalize(carConfiguration);
            return controller;
        }

        public static IIndividual DoCrossover(IIndividual mother, IIndividual father, Random random)
        {
            var motherWeights = mother.Network.GetFlattenedWeights();
            var fatherWeights = mother.Network.GetFlattenedWeights();

            int crossoverPosition = random.Next(1, motherWeights.Length - 1);


            var offsprintWeights = new List<float>(motherWeights.Length);

            for (int i = 0; i < motherWeights.Length; i++)
            {
                offsprintWeights.Add(i < crossoverPosition ? motherWeights[i] : fatherWeights[i]);
            }

            var offspring = new CarAI(new int[] { 12, 12, 12, 4 }, random, offsprintWeights);

            offspring.Initalize(((CarAI)mother).Configuration);

            return offspring;
        }
    }
}
