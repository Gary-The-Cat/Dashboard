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
using Shared.NeuralNetworks;
using Shared.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SelfDriving.Screens.HumanAssistedTraining
{
    public class HumanAssistedTrainingScreen : Screen
    {
        private TrackSelectionScreen trackSelection;
        private RacingSimulation racingSimulation;
        private RacingSimulationVisualization racingSimulationVisualization;
        private SelfDrivingTestScreen selfDrivingTestScreen;

        private GameState gameState;
        private Track currentTrack;
        private List<Button> buttons;

        private Button toggleCapture;
        private Button samplesCaptured;
        private CarHuman humanAi;
        private MLPNeuralNetwork network;

        private IApplication application;
        private IApplicationInstance applicationInstance;
        private Screen parentScreen;

        public HumanAssistedTrainingScreen(
            IApplication application,
            IApplicationInstance applicationInstance,
            Screen parentScreen) 
            : base(application.Configuration, applicationInstance)
        {
            this.application = application;
            this.applicationInstance = applicationInstance;
            this.parentScreen = parentScreen;

            racingSimulation = new RacingSimulation(application);
            racingSimulationVisualization = new RacingSimulationVisualization(application, applicationInstance, racingSimulation);

            trackSelection = new TrackSelectionScreen(
                application.Configuration,
                applicationInstance,
                "Resources/Tracks");

            buttons = new List<Button>();

            RegisterCallbacks();

            trackSelection.OnTrackSelected = OnTrackSelected;

            gameState = GameState.TrackSelection;

            // Create our human player
            humanAi = new CarHuman(true, 20);
            humanAi.Initalize(new CarConfiguration());

            this.PopulateButtons();
        }

        private void PopulateButtons()
        {
            toggleCapture = new Button("Stop Capture", new Vector2f(20, 20), () =>
            {
                if (humanAi.IsCapturing())
                {
                    ToggleCaptureOff();
                }
                else
                {
                    ToggleCaptureOn();
                }
            }, HorizontalAlignment.Left);

            buttons.Add(toggleCapture);

            samplesCaptured = new Button(
                "0", 
                new Vector2f(this.application.Configuration.Width - 20, 20), 
                () => { }, 
                HorizontalAlignment.Right);

            buttons.Add(samplesCaptured);

            buttons.Add(new Button("Reset", new Vector2f(20, 75), () =>
            {
                humanAi.ResetCapture();
                application.NotificaitonService.ShowToast(
                    ToastType.Info,
                    "Capture Data Cleared");
            }, HorizontalAlignment.Left));

            buttons.Add(new Button("Train", new Vector2f(20, 125), () =>
            {
                StartTraining();
            }, HorizontalAlignment.Left));

            buttons.Add(new Button("Test", new Vector2f(20, 175), () =>
            {
                application.NotificaitonService.ShowToast(
                    ToastType.Info,
                    "Starting Test...");

                StartTesting();
            }, HorizontalAlignment.Left));

            buttons.Add(new Button("Export", new Vector2f(20, application.Configuration.Height - 65), () =>
            {
                SaveNetwork();

                application.NotificaitonService.ShowToast(
                    ToastType.Info,
                    "Saved");
            }, HorizontalAlignment.Left));
        }

        private void SaveNetwork()
        {
            if (!EnsureNetworkExists())
            {
                return;   
            }

            var fileBasedNetwork = network.GetFileRepresentation();
            var baseFileName = $"{Directory.GetCurrentDirectory()}\\Network_{DateTime.Now:YY_DD_MM_hh.mm.ss}";
            var saveLocation = $"{baseFileName}.mlpnn";
            File.WriteAllText(saveLocation, fileBasedNetwork);
        }

        private void StartTesting()
        {
            if (!EnsureNetworkExists())
            {
                return;
            }

            SetInactive();

            selfDrivingTestScreen = new SelfDrivingTestScreen(
                application, 
                applicationInstance, 
                () =>
                {
                    SetActive();
                    trackSelection.SetInactive();
                });

            selfDrivingTestScreen.Initialize(currentTrack, network);

            applicationInstance.AddChildScreen(selfDrivingTestScreen, this);

            selfDrivingTestScreen.Start();
            selfDrivingTestScreen.SetActive();
        }

        private bool EnsureNetworkExists()
        {
            if (network == null)
            {
                application.NotificaitonService.ShowToast(
                    ToastType.Error,
                    "You must train a network first!");

                return false;
            }

            return true;
        }

        private void StartTraining()
        {
            var (inputData, expectedOutputData) = GetReducedDataset(
                humanAi.StateInputMeasurements.ToList(), 
                humanAi.StateOutputMeasurements.ToList()); 

            for(int i = 0; i < inputData.Count(); i++)
            {
                if (inputData[i].All(d => d == 0))
                {
                    inputData.RemoveAt(i);
                    expectedOutputData.RemoveAt(i);

                    i--;
                }
            }

            application.NotificaitonService.ShowToast(
                ToastType.Info,
                "Training Started...");

            // Right now the training happens so fast, not sure if we need to be reporting progress
            network = NeuralNetworkHelper.GetTrainedNetwork(inputData, expectedOutputData, (c, t) => { });

            application.NotificaitonService.ShowToast(
                ToastType.Info,
                "Training Complete.");
        }

        private (List<float[]>, List<float[]>) GetReducedDataset(List<float[]> inputData, List<float[]> expectedOutputData)
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

        private void ToggleCaptureOn()
        {
            humanAi.StartCapture();

            toggleCapture.Text = "Stop Capture";
            application.NotificaitonService.ShowToast(
                ToastType.Info,
                "Capture Started...");
        }

        private void ToggleCaptureOff()
        {
            humanAi.StopCapture();

            toggleCapture.Text = "Start Capture";
            application.NotificaitonService.ShowToast(
                ToastType.Info,
                "Capture Stopped");
        }

        private void OnMousePress(MouseClickEventArgs args)
        {
            buttons.ForEach(b => b.TryClick(args));
        }

        private void RegisterCallbacks()
        {
            RegisterKeyboardCallback(new KeyPressCallbackEventArgs(Keyboard.Key.R), ResetSimulation);
            RegisterKeyboardCallback(new KeyPressCallbackEventArgs(Keyboard.Key.M), SetTrackSelection);
            RegisterMouseClickCallback(new MouseClickCallbackEventArgs(Mouse.Button.Left), OnMousePress);
            RegisterJoystickCallback(new JoystickCallbackEventArgs(7), ResetSimulation);
        }

        private void OnTrackSelected(Track track)
        {
            // Toggle our game state to now be racing.
            gameState = GameState.Racing;

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

        private void ResetSimulation(object args)
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
                    samplesCaptured.Text = $"{humanAi.SamplesCaptured}";
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
    }
}
