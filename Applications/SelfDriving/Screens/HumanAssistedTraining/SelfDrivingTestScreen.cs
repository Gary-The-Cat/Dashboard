using SelfDriving.Agents;
using SelfDriving.Interfaces;
using SelfDriving.Shared;
using SFML.Graphics;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.Interfaces;
using Shared.NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SelfDriving.Screens.HumanAssistedTraining
{
    public class SelfDrivingTestScreen : Screen
    {
        private Random random;
        private RacingSimulation simulation;
        private RacingSimulationVisualization simulationVisualization;
        private IApplication application;
        private IApplicationInstance applicationInstance;
        private Action returnToTrainScreen;

        public SelfDrivingTestScreen(
            IApplication application,
            IApplicationInstance applicationInstance,
            Action returnToTrainScreen) 
            : base(application.Configuration, applicationInstance)
        {
            this.application = application;
            this.applicationInstance = applicationInstance;
            this.returnToTrainScreen = returnToTrainScreen;

            random = new Random();

            RegisterKeyboardCallback(new KeyPressCallbackEventArgs(SFML.Window.Keyboard.Key.B), ReturnToTrainScreen);
        }

        private void ReturnToTrainScreen(KeyboardEventArgs obj)
        {
            SetInactive();
            returnToTrainScreen?.Invoke();
        }

        public void Initialize(Track track, MLPNeuralNetwork controller)
        {
            var carAi = new CarAI(controller.GetStructure(), random, controller.GetFlattenedWeights().ToList());

            // Load the simulation
            simulation = new RacingSimulation(application);
            simulation.SetTrack(track);

            // Initialize the simulation
            simulation.InitializeCars(new List<ICarAI> { carAi });

            // If the visualization is turned on, create it, set the track and add the cars.
            simulationVisualization = new RacingSimulationVisualization(application, applicationInstance, simulation);
            simulationVisualization.SetTrack(track);
            simulationVisualization.InitializeCars(simulation.GetCars());
        }

        public override void OnUpdate(float deltaT)
        {
            base.OnUpdate(deltaT);
            simulation.OnUpdate(deltaT);
            simulationVisualization.OnUpdate(deltaT);
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);
            simulationVisualization.OnRender(target);
        }
    }
}
