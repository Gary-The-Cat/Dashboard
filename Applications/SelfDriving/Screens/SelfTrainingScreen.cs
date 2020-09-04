using SelfDriving.Agents;
using SelfDriving.DataStructures;
using SelfDriving.Helpers;
using SelfDriving.Interfaces;
using SelfDriving.Shared;
using SFML.Graphics;
using Shared.Core;
using Shared.GeneticAlgorithms;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SelfDriving.Screens
{
    public class SelfTrainingScreen : Screen
    {
        private IApplication application;

        private TrainState trainingState;

        private RacingSimulation simulation;

        private RacingSimulationVisualization simulationVisualization;

        private List<Track> tracks;

        private int currentTrackIndex = 0;

        private Track currentTrack => tracks[currentTrackIndex];

        private GenericGA genericAlgorithm;

        private static Random random;

        private bool IsVisualizationEnabled => simulationVisualization != null;

        private bool TrackEvaluationFinished => simulation.GetCars().All(c => !c.IsRunning);

        public SelfTrainingScreen(IApplication application, bool enableVisualization = true) : base(application.Configuration)
        {
            this.application = application;
            random = new Random();

            // Create the GA
            genericAlgorithm = this.CreateGeneticAlgorithm();

            // Load the tracks used for evaluation
            tracks = TrackHelper.LoadTrackFiles("Resources/Tracks");

            // Spawn the initial population
            genericAlgorithm.SpawnPopulation();

            // Load the simulation
            simulation = new RacingSimulation(application);
            simulation.SetTrack(currentTrack);

            // Initialize the simulation
            simulation.InitializeCars(genericAlgorithm.GetPopulation().Cast<ICarAI>());

            // Add the metrics we will use to judge our cars fitness
            simulation.GetCars().ForEach(car => this.AddFitnessMetrics(car));

            // If the visualization is turned on, create it, set the track and add the cars.
            if (enableVisualization)
            {
                simulationVisualization = new RacingSimulationVisualization(application, simulation);
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

            simulation.InitializeCars(genericAlgorithm.GetPopulation().Cast<ICarAI>());

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
