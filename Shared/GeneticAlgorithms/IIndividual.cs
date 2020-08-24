using Shared.NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.GeneticAlgorithms
{
    public interface IIndividual
    {
        MLPNeuralNetwork Network { get; set; }

        void Mutate();

        double GetFitness();

        IIndividual Clone();
    }
}
