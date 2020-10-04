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

        public static (List<float[]>, List<float[]>) ReduceDataset(List<float[]> inputData, List<float[]> expectedOutputData)
        {
            var accellerateData = new List<(float[], float[])>();
            var brakeData = new List<(float[], float[])>();
            var turnLeftData = new List<(float[], float[])>();
            var turnRightData = new List<(float[], float[])>();

            for (int i = 0; i < inputData.Count; i++)
            {
                if (expectedOutputData[i][0] > 0)
                {
                    accellerateData.Add((inputData[i], expectedOutputData[i]));
                }

                if (expectedOutputData[i][1] > 0)
                {
                    turnLeftData.Add((inputData[i], expectedOutputData[i]));
                }

                if (expectedOutputData[i][2] > 0)
                {
                    turnRightData.Add((inputData[i], expectedOutputData[i]));
                }

                if (expectedOutputData[i][3] > 0)
                {
                    brakeData.Add((inputData[i], expectedOutputData[i]));
                }
            }

            var minCount = Math.Min(Math.Min(
                accellerateData.Count(),
                turnLeftData.Count()),
                turnRightData.Count());

            var resultInputData = new List<float[]>();
            var resultExpectedOutputData = new List<float[]>();

            for (int i = 0; i < minCount; i++)
            {
                resultInputData.Add(accellerateData[i].Item1);
                resultExpectedOutputData.Add(accellerateData[i].Item2);

                resultInputData.Add(turnLeftData[i].Item1);
                resultExpectedOutputData.Add(turnLeftData[i].Item2);

                resultInputData.Add(turnRightData[i].Item1);
                resultExpectedOutputData.Add(turnRightData[i].Item2);
            }

            return (resultInputData, resultExpectedOutputData);
        }
    }
}
