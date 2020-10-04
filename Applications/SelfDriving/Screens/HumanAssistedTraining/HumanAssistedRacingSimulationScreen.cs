using SelfDriving.Agents;
using SelfDriving.Interfaces;
using SelfDriving.Shared;
using SelfDriving.Shared.RaceSimulation;
using Shared.Core.Hierarchy;
using Shared.Interfaces;
using System.Collections.Generic;

namespace SelfDriving.Screens.HumanAssistedTraining
{
    public class HumanAssistedRacingSimulationScreen : StackedScreen
    {
        public RacingSimulationScreen RacingSimulationScreen { get; private set; }

        public HumanAssistedTrainingHudScreen HumanAssistedHud { get; private set; }

        private CarHuman humanCar;

        public HumanAssistedRacingSimulationScreen(
            IApplication application,
            IApplicationInstance applicationInstance) : base(application, applicationInstance)
        {
            // Create our human driven car
            humanCar = new CarHuman(true, 100);

            RacingSimulationScreen = new RacingSimulationScreen(
                Application, 
                ParentApplication,
                new List<ICarController> { humanCar });

            HumanAssistedHud = new HumanAssistedTrainingHudScreen(
                Application, 
                ParentApplication,
                RacingSimulationScreen);

            AddScreen(RacingSimulationScreen);

            AddScreen(HumanAssistedHud);
        }

        public void OnTrackSelected(Track track)
        {
            RacingSimulationScreen.OnTrackSelected(track);
        }
    }
}
