using SelfDriving.Agents;
using SelfDriving.Interfaces;
using SelfDriving.Shared;
using SelfDriving.Shared.RaceSimulation;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.Interfaces;
using Shared.Menus;
using Shared.NeuralNetworks;
using System;
using System.Collections.Generic;

namespace SelfDriving.Screens.HumanAssistedTraining
{
    public class SelfDrivingTestScreen : Screen
    {
        private Random random;
        private RacingSimulationLogic simulation;
        private RacingSimulationVisualization simulationVisualization;
        private Button backButton;

        public SelfDrivingTestScreen(
            IApplication application,
            IApplicationInstance applicationInstance) 
            : base(application, applicationInstance)
        {
            random = new Random();

            backButton = new Button(
                "Back",
                new Vector2f(20, 20),
                ParentApplication.GoBack,
                HorizontalAlignment.Left);

            RegisterMouseClickCallback(new MouseClickCallbackEventArgs(SFML.Window.Mouse.Button.Left), OnMouseClick);
        }

        public void Initialize(Track track, MLPNeuralNetwork controller)
        {
            var carAi = new CarAI(random, controller);

            // Load the simulation
            simulation = new RacingSimulationLogic(Application);
            simulation.SetTrack(track);

            // Add the 1:1 trained AI to the list of cars
            var cars = new List<ICarController>() { carAi };

            // Spawn 100 mutated versions of our AI
            for(int i = 0; i < 100; i++)
            {
                var car = carAi.Clone();
                car.Mutate();
                cars.Add((CarAI)car);
            }

            // Initialize the simulation
            simulation.SetCars(cars);
            simulation.ResetCars();

            // If the visualization is turned on, create it, set the track and add the cars.
            simulationVisualization = new RacingSimulationVisualization(simulation);
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

            target.SetView(ParentApplication.DefaultView);
            backButton.OnRender(target);
        }

        private void OnMouseClick(MouseClickEventArgs obj)
        {
            backButton.TryClick(obj);
        }

        public override void SetActive()
        {
            base.SetActive();
        }
    }
}
