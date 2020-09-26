using SelfDriving.Agents;
using SelfDriving.DataStructures;
using SelfDriving.Interfaces;
using SelfDriving.Screens.TrackSelection;
using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.Interfaces;
using Shared.Menus;
using Shared.Notifications;
using System;
using System.Collections.Generic;

namespace SelfDriving.Screens.HumanAssistedTraining
{
    public class HumanAssistedTrainingScreen : Screen
    {
        private TrackSelectionScreen trackSelection;
        private RacingSimulation racingSimulation;
        private RacingSimulationVisualization racingSimulationVisualization;

        private GameState gameState;
        private Track currentTrack;
        private List<Button> buttons;

        private IApplication application;

        public HumanAssistedTrainingScreen(
            IApplication application,
            IApplicationInstance applicationInstance) 
            : base(application.Configuration, applicationInstance)
        {
            this.application = application;

            racingSimulation = new RacingSimulation(application);
            racingSimulationVisualization = new RacingSimulationVisualization(application, applicationInstance, racingSimulation);

            trackSelection = new TrackSelectionScreen(
                application.Configuration,
                applicationInstance,
                "Resources/Tracks");

            trackSelection.SetInactive();

            AddChildScreen(trackSelection);

            buttons = new List<Button>();

            RegisterCallbacks();

            trackSelection.OnTrackSelected = OnTrackSelected;

            gameState = GameState.TrackSelection;

            this.PopulateButtons();
        }

        private void PopulateButtons()
        {
            buttons.Add(new Button("Draw", new Vector2f(20, 20), () =>
            {
                application.NotificaitonService.ShowToast(
                    ToastType.Info,
                    "Drawing Lines Enabled");
            }, HorizontalAlignment.Left));
        }
        private void OnMousePress(MouseClickEventArgs args)
        {
            buttons.ForEach(b =>
            {
                if (b.GetGlobalBounds().Contains(args.Args.X, args.Args.Y))
                {
                    b.OnClick();

                    args.IsHandled = true;
                }
            });
        }

        private void RegisterCallbacks()
        {
            RegisterKeyboardCallback(new KeyPressCallbackEventArgs(Keyboard.Key.R), ResetSimulation);
            RegisterKeyboardCallback(new KeyPressCallbackEventArgs(Keyboard.Key.M), SetTrackSelection);
            RegisterMouseClickCallback(new MouseClickCallbackEventArgs(Mouse.Button.Left), OnMousePress);
            //RegisterJoystickCallback(application.Window, 7, ResetSimulation);
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

        private void SetTrackSelection(KeyboardEventArgs args)
        {
            ((RenderWindow)application.Window).SetView(application.GetDefaultView());
            this.trackSelection.SetActive();
            this.gameState = GameState.TrackSelection;
        }

        private void ResetSimulation(KeyboardEventArgs args)
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
                    buttons.ForEach(b => b.OnUpdate());
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
                    target.SetView(application.GetDefaultView());
                    buttons.ForEach(b => b.OnRender(target));
                    break;
            }
        }

        public override void SetActive()
        {
            base.SetActive();

            trackSelection.SetActive();
        }

        public override void SetInactive()
        {
            base.SetInactive();

            trackSelection.SetInactive();
        }
    }
}
