using Ninject;
using Ninject.Parameters;
using SelfDriving.Agents;
using SelfDriving.Interfaces;
using SelfDriving.Shared;
using SelfDriving.Shared.RaceSimulation;
using Shared.Core.Hierarchy;
using Shared.Interfaces.Services;
using System.Collections.Generic;

namespace SelfDriving.Screens.HumanAssistedTraining
{
    public class HumanAssistedRacingSimulationScreen : StackedScreen
    {
        public RacingSimulationScreen RacingSimulationScreen { get; private set; }

        public HumanAssistedTrainingHudScreen HumanAssistedHud { get; private set; }

        private CarHuman humanCar;

        public HumanAssistedRacingSimulationScreen(
            IApplicationService appService)
        {
            // Create our human driven car
            humanCar = new CarHuman(true, 100);

            RacingSimulationScreen = appService.Kernel.Get<RacingSimulationScreen>(
                new ConstructorArgument("carControllers", new List<ICarController> { humanCar }));

            HumanAssistedHud = appService.Kernel.Get<HumanAssistedTrainingHudScreen>(
                new ConstructorArgument("racingSimulationScreen", RacingSimulationScreen));

            AddScreen(RacingSimulationScreen);

            AddScreen(HumanAssistedHud);
        }

        public void OnTrackSelected(Track track)
        {
            RacingSimulationScreen.OnTrackSelected(track);
        }
    }
}
