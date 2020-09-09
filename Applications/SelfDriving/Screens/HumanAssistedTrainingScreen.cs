using SelfDriving.Agents;
using SelfDriving.DataStructures;
using SelfDriving.Interfaces;
using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Interfaces;
using System.Collections.Generic;

namespace SelfDriving.Screens
{
    public class HumanAssistedTrainingScreen : Screen
    {
        private TrackSelectionVisual trackSelection;
        private RacingSimulation racingSimulation;
        private RacingSimulationVisualization racingSimulationVisualization;

        private GameState gameState;
        private Track currentTrack;

        private IApplication application;

        public HumanAssistedTrainingScreen(
            IApplication application,
            IApplicationInstance applicationInstance) 
            : base(application.Configuration, applicationInstance)
        {
            this.application = application;

            racingSimulation = new RacingSimulation(application);
            racingSimulationVisualization = new RacingSimulationVisualization(application, applicationInstance, racingSimulation);

            trackSelection = new TrackSelectionVisual(
                application,
                new Vector2f(0, 0),
                "Resources/Tracks");

            RegisterKeyboardCallback(application.Window, Keyboard.Key.R, ResetSimulation);
            RegisterKeyboardCallback(application.Window, Keyboard.Key.M, SetTrackSelection);
            RegisterJoystickCallback(application.Window, 7, ResetSimulation);

            trackSelection.OnTrackSelected = OnTrackSelected;

            gameState = GameState.TrackSelection;
        }

        private void OnTrackSelected(Track track)
        {
            // Toggle our game state to now be racing.
            gameState = GameState.Racing;

            // Create our human player
            var humanAi = new CarHuman(true, 100);
            humanAi.Initalize(new CarConfiguration());

            // Add the car into the simulation
            racingSimulation.SetTrack(track);
            racingSimulation.InitializeCars(new List<ICarAI> { humanAi });

            // Add all cars to the visualization
            racingSimulationVisualization.SetTrack(track);
            racingSimulationVisualization.InitializeCars(racingSimulation.GetCars());

            // Update the current track
            currentTrack = track;
        }

        private void SetTrackSelection()
        {
            ((RenderWindow)application.Window).SetView(application.GetDefaultView());
            this.trackSelection.SetActive(true);
            this.gameState = GameState.TrackSelection;
        }

        private void ResetSimulation()
        {
            racingSimulation.Reset();
            racingSimulationVisualization.Reset();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            switch (gameState)
            {
                case GameState.Racing:
                    racingSimulation.OnUpdate(dt);
                    racingSimulationVisualization.OnUpdate(dt);
                    break;
            }
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);

            switch (gameState)
            {
                case GameState.TrackSelection:
                    trackSelection.OnRender(target);
                    break;
                case GameState.Racing:
                    racingSimulationVisualization.OnRender(target);
                    break;
            }
        }
    }
}
