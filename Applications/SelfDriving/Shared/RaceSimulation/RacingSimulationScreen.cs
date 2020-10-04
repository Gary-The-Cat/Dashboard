using SelfDriving.Agents;
using SelfDriving.Interfaces;
using SFML.Graphics;
using SFML.Window;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SelfDriving.Shared.RaceSimulation
{
    public class RacingSimulationScreen : Screen
    {
        private RacingSimulationLogic racingSimulationLogic;

        private RacingSimulationVisualization racingSimulationVisualization;

        private List<ICarController> carControllers;

        public Track CurrentTrack { get; private set; }

        public RacingSimulationScreen(
            IApplication application,
            IApplicationInstance applicationInstance,
            IEnumerable<ICarController> carControllers)
            : base(application, applicationInstance)
        {
            racingSimulationLogic = new RacingSimulationLogic(application);
            racingSimulationVisualization = new RacingSimulationVisualization(racingSimulationLogic);
            this.carControllers = carControllers.ToList();

            RegisterKeyboardCallback(new KeyPressCallbackEventArgs(Keyboard.Key.R), OnResetRequested);
            RegisterJoystickCallback(new JoystickCallbackEventArgs(7), OnResetRequested);
        }

        private void OnResetRequested(object _)
        {
            racingSimulationLogic.Reset();
            racingSimulationVisualization.Reset();
        }

        public override void InitializeScreen()
        {
            base.InitializeScreen();

            racingSimulationLogic.SetCars(carControllers);
        }

        public override void OnUpdate(float dt)
        {
            racingSimulationLogic.OnUpdate(dt);
            racingSimulationVisualization.OnUpdate(dt);

            base.OnUpdate(dt);
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);

            racingSimulationVisualization.OnRender(target);
        }

        public void OnTrackSelected(Track track)
        {
            CurrentTrack = track;

            racingSimulationLogic.SetTrack(track);
            racingSimulationLogic.ResetCars();

            // Add all cars to the visualization
            racingSimulationVisualization.SetTrack(track);
            racingSimulationVisualization.InitializeCars(racingSimulationLogic.GetCars());
        }

        public IEnumerable<Car> GetCars()
        {
            return racingSimulationLogic.GetCars();
        }

        public IEnumerable<ICarController> GetCarControllers()
        {
            return racingSimulationLogic.GetCars().Select(c => c.Controller);
        }
    }
}
