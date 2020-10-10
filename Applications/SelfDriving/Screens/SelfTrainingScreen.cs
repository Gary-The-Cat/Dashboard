﻿using Ninject;
using SelfDriving.Agents;
using SelfDriving.DataStructures;
using SelfDriving.Helpers;
using SelfDriving.Interfaces;
using SelfDriving.Shared;
using SelfDriving.Shared.RaceSimulation;
using SFML.Graphics;
using Shared.Core;
using Shared.GeneticAlgorithms;
using Shared.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SelfDriving.Screens
{
    public class SelfTrainingScreen : Screen
    {
        private TrainState trainingState;

        private RacingSimulationLogic simulation;

        private RacingSimulationVisualization simulationVisualization;

        private List<Track> tracks;

        private int currentTrackIndex = 0;

        private Track currentTrack => tracks[currentTrackIndex];

        private GenericGA genericAlgorithm;

        private static Random random;

        private bool IsVisualizationEnabled => simulationVisualization != null;

        private bool TrackEvaluationFinished => simulation.GetCars().All(c => !c.IsRunning);

        public SelfTrainingScreen(
            IApplicationService appService,
            bool enableVisualization = true)
        {
            random = new Random();


            List<float> seed = null;
            var seedFile = "Resources/CarAi/AI_2.mlpnn";
            if (File.Exists(seedFile))
            {
                var seedText = File.ReadAllText(seedFile);
                seed = seedText.Split(',').Select(s => float.Parse(s)).ToList();
            }

            // Create the GA
            genericAlgorithm = this.CreateGeneticAlgorithm(seed);

            // Load the tracks used for evaluation
            tracks = TrackHelper.LoadTrackFiles("Resources/Tracks");

            // Spawn the initial population
            genericAlgorithm.SpawnPopulation();

            // Load the simulation
            simulation = appService.Kernel.Get<RacingSimulationLogic>();
            simulation.SetTrack(currentTrack);

            // Initialize the simulation
            simulation.SetCars(genericAlgorithm.GetPopulation().Cast<ICarController>());
            simulation.ResetCars();

            // Add the metrics we will use to judge our cars fitness
            simulation.GetCars().ForEach(car => this.AddFitnessMetrics(car));

            // If the visualization is turned on, create it, set the track and add the cars.
            if (enableVisualization)
            {
                simulationVisualization = appService.Kernel.Get<RacingSimulationVisualization>();
                simulationVisualization.SetTrack(currentTrack);
                simulationVisualization.InitializeCars(simulation.GetCars());
            }
        }

        private void AddFitnessMetrics(Car car)
        {
            car.AddFitnessMetric(FitnessMetrics.DistanceMetric);
            car.AddFitnessMetric(FitnessMetrics.TimeAliveMetric);
            car.AddFitnessMetric(FitnessMetrics.CheckpointsPassedMetric);
        }

        private GenericGA CreateGeneticAlgorithm(List<float> seed = null)
        {
            var geneticAlgorithm = new GenericGA();

            var carConfiguration = new CarConfiguration();

            geneticAlgorithm.CreateIndividual = () =>
            {
                return GAHelpers.SpawnCarController(carConfiguration, random, seed);
            };

            geneticAlgorithm.CrossoverIndividuals = (a, b) =>
            {
                return GAHelpers.DoCrossover(a, b, random);
            };

            geneticAlgorithm.CrossoverEnabled = true;

            geneticAlgorithm.MutationEnabled = true;

            geneticAlgorithm.MutateParentsAsChildren = true;

            geneticAlgorithm.PopulationCount = 100;

            geneticAlgorithm.SpawnPopulation();

            return geneticAlgorithm;
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            switch (trainingState)
            {
                case TrainState.Evaluating:
                    OnEvaluatingUpdate(dt);
                    break;
                case TrainState.Breeding:
                    OnBreedingUpdate();
                    break;
            }
        }

        private void OnBreedingUpdate()
        {
            // Perform one generation of our GA
            genericAlgorithm.DoGeneration();

            simulation.SetCars(genericAlgorithm.GetPopulation().Cast<ICarController>());
            simulation.ResetCars();

            // Add the metrics we will use to judge our cars fitness
            simulation.GetCars().ForEach(car => this.AddFitnessMetrics(car));

            // Reset our simulation
            currentTrackIndex = 0;
            simulation.SetTrack(currentTrack);
            simulation.Reset();

            // Reset the visualisation
            if (IsVisualizationEnabled)
            {
                // Reset all cars
                simulationVisualization.InitializeCars(simulation.GetCars());

                // Reset all track visuals - not the most efficient but works really clean
                simulationVisualization.SetTrack(currentTrack);
                simulationVisualization.Reset();
            }


            trainingState = TrainState.Evaluating;
        }

        private void OnEvaluatingUpdate(float dt)
        {
            simulation.OnUpdate(dt);

            if (IsVisualizationEnabled)
            {
                simulationVisualization.OnUpdate(dt);
            }

            if (TrackEvaluationFinished)
            {
                // Increment to the next track
                currentTrackIndex++;

                // All evaluation has finished, move onto breeding
                if (currentTrackIndex == tracks.Count)
                {
                    this.trainingState = TrainState.Breeding;
                    return;
                }

                // Will reset all cars to the initial position for the track provided
                simulation.SetTrack(currentTrack);
                simulation.Reset();

                // Resets all car visuals & sets new track visualization
                if (IsVisualizationEnabled)
                {
                    simulationVisualization.SetTrack(currentTrack);
                    simulationVisualization.Reset();
                }
            }
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);

            if (IsVisualizationEnabled)
            {
                simulationVisualization.OnRender(target);
            }
        }
    }
}
