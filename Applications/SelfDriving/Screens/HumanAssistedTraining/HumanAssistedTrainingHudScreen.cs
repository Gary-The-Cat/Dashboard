using Ninject;
using SelfDriving.Agents;
using SelfDriving.Shared.RaceSimulation;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using Shared.Menus;
using Shared.NeuralNetworks;
using Shared.Notifications;
using Shared.ScreenConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SelfDriving.Screens.HumanAssistedTraining
{
    public class HumanAssistedTrainingHudScreen : Screen
    {
        private readonly List<Button> buttons;
        private readonly RacingSimulationScreen racingSimulationScreen;
        private SelfDrivingTestScreen selfDrivingTestScreen;

        private MLPNeuralNetwork network;
        private CarHuman humanCar;
        private Button toggleCapture;
        private Button samplesCaptured;

        private IApplicationManager appManager;
        private INotificationService notificationService;
        private IApplicationService appService;

        public HumanAssistedTrainingHudScreen(
            IEventService eventService,
            INotificationService notificationService,
            IApplicationManager appManager,
            IApplicationService appService,
            RacingSimulationScreen racingSimulationScreen)
        {
            this.appManager = appManager;
            this.notificationService = notificationService;
            this.appService = appService;

            buttons = new List<Button>();

            this.racingSimulationScreen = racingSimulationScreen;

            PopulateButtons();

            eventService.RegisterMouseClickCallback(this.Id, new MouseClickCallbackEventArgs(Mouse.Button.Left), OnMousePress);
        }

        public override void InitializeScreen()
        {
            base.InitializeScreen();

            humanCar = racingSimulationScreen.GetCarControllers().First() as CarHuman;
        }

        private void OnMousePress(MouseClickEventArgs eventArgs)
        {
            buttons.ForEach(button => button.TryClick(eventArgs));
        }

        private void PopulateButtons()
        {
            toggleCapture = new Button(
                "Stop Capture", 
                new Vector2f(20, 20), 
                () => ToggleCapture(), 
                HorizontalAlignment.Left);

            buttons.Add(toggleCapture);

            samplesCaptured = new Button(
                "0",
                new Vector2f(appManager.GetScreenConfiguration().Width - 20, 20),
                () => { },
                HorizontalAlignment.Right);

            buttons.Add(samplesCaptured);

            buttons.Add(new Button("Reset", new Vector2f(20, 75), () =>
            {
                humanCar.ResetCapture();
                notificationService.ShowToast(
                    ToastType.Info,
                    "Capture Data Cleared");
            }, HorizontalAlignment.Left));

            buttons.Add(new Button("Train", new Vector2f(20, 125), () =>
            {
                StartTraining();
            }, HorizontalAlignment.Left));

            buttons.Add(new Button("Test", new Vector2f(20, 175), () =>
            {
                notificationService.ShowToast(
                    ToastType.Info,
                    "Starting Test...");

                StartTesting();
            }, HorizontalAlignment.Left));

            buttons.Add(new Button("Export", new Vector2f(20, appManager.GetScreenConfiguration().Height - 65), () =>
            {
                SaveNetwork();

                notificationService.ShowToast(
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

            if(selfDrivingTestScreen == null)
            {
                selfDrivingTestScreen = appService.Kernel.Get<SelfDrivingTestScreen>();

                appManager.AddChildScreen(selfDrivingTestScreen);
            }
            else
            {
                appManager.SetActiveScreen(selfDrivingTestScreen);
            }

            selfDrivingTestScreen.Initialize(racingSimulationScreen.CurrentTrack, network);
        }

        private bool EnsureNetworkExists()
        {
            if (network == null)
            {
                notificationService.ShowToast(
                    ToastType.Error,
                    "You must train a network first!");

                return false;
            }

            return true;
        }

        private void StartTraining()
        {
            var (inputData, expectedOutputData) = NeuralNetworkHelper.ReduceDataset(
                humanCar.StateInputMeasurements.ToList(),
                humanCar.StateOutputMeasurements.ToList());

            for (int i = 0; i < inputData.Count(); i++)
            {
                if (inputData[i].All(d => d == 0))
                {
                    inputData.RemoveAt(i);
                    expectedOutputData.RemoveAt(i);

                    i--;
                }
            }

            notificationService.ShowToast(
                ToastType.Info,
                "Training Started...");

            // Right now the training happens so fast, not sure if we need to be reporting progress
            network = NeuralNetworkHelper.GetTrainedNetwork(inputData, expectedOutputData, (c, t) => { });

            notificationService.ShowToast(
                ToastType.Info,
                "Training Complete.");
        }

        private void ToggleCapture()
        {
            humanCar.ToggleCapture();

            toggleCapture.Text = humanCar.IsCapturing() ? "Stop Capture" : "Start Capture";
        }

        public override void OnUpdate(float deltaT)
        {
            base.OnUpdate(deltaT);

            samplesCaptured.Text = humanCar.SamplesCaptured.ToString();
        }

        public override void OnRender(RenderTarget target)
        {
            target.SetView(appManager.GetDefaultView());

            buttons.ForEach(button => button.OnRender(target));
        }
    }
}