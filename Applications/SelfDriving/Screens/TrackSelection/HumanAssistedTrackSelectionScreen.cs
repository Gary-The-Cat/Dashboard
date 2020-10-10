using Ninject;
using Ninject.Parameters;
using SelfDriving.Screens.HumanAssistedTraining;
using SelfDriving.Shared;
using SFML.Graphics;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Interfaces;
using Shared.Interfaces.Services;

namespace SelfDriving.Screens.TrackSelection
{
    public class HumanAssistedTrackSelectionScreen : Screen
    {
        private TrackSelectionVisual trackSelectionVisual;

        private HumanAssistedRacingSimulationScreen raceSimulationScreen;

        private IApplicationManager appManager;

        private IApplicationService appService;

        public HumanAssistedTrackSelectionScreen(
            IApplicationService appService,
            IApplicationManager appManager,
            IEventService eventService)
        {
            this.appService = appService;
            this.appManager = appManager;

            trackSelectionVisual = appService.Kernel.Get<TrackSelectionVisual>(
                new ConstructorArgument("trackDirectory", "Resources\\Tracks"));

            eventService.RegisterMouseClickCallback(this.Id, new MouseClickCallbackEventArgs(SFML.Window.Mouse.Button.Left), trackSelectionVisual.OnMousePress);

            trackSelectionVisual.OnTrackSelected = OnTrackSelected;
        }

        public override void OnRender(RenderTarget target)
        {
            trackSelectionVisual.OnRender(target);
        }

        private void OnTrackSelected(Track track)
        {
            if (raceSimulationScreen == null)
            {
                raceSimulationScreen = appService.Kernel.Get<HumanAssistedRacingSimulationScreen>();

                appManager.AddChildScreen(raceSimulationScreen);
            }
            else
            {
                appManager.SetActiveScreen(raceSimulationScreen);
            }

            raceSimulationScreen.OnTrackSelected(track);
        }
    }
}
