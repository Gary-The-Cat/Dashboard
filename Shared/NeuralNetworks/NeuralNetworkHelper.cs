using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.NeuralNetworks
{
    public static class NeuralNetworkHelper
    {
        public static MLPNeuralNetwork GetTrainedNetwork(
            List<float[]> inputData, 
            List<float[]> expectedOutputData,
            Action<int, int> reportProgress)
        {
            if (!inputData.Any() || !expectedOutputData.Any())
            {
                throw new Exception("Input data and expected output data must contain values.");
            }

            if (inputData.Count() != expectedOutputData.Count())
            {
                throw new Exception("Input data and expected output data are not the same length.");
            }

            var inputNodes = inputData.First().Length;
            var outputNodes = expectedOutputData.First().Length;


            var innerLayers = 2;
            var hiddenLayerDepth = 16;

            // Inner Layer + Input + Output = 3
            var layers = new int[innerLayers + 2];

            layers[0] = inputNodes;
            for (int i = 1; i < innerLayers + 1; i++)
            {
                layers[i] = hiddenLayerDepth;
            }
            layers[innerLayers + 1] = outputNodes;

            // Create the NN.
            var network = new MLPNeuralNetwork(layers);

            // Train the network with our known good data.
            network.Train(inputData, expectedOutputData, reportProgress);

            return network;
        }
    }
}
