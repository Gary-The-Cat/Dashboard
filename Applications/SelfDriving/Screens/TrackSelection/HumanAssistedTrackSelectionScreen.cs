using SelfDriving.Screens.HumanAssistedTraining;
using SelfDriving.Shared;
using SFML.Graphics;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Interfaces;

namespace SelfDriving.Screens.TrackSelection
{
    public class HumanAssistedTrackSelectionScreen : Screen
    {
        private TrackSelectionVisual trackSelectionVisual;

        private HumanAssistedRacingSimulationScreen raceSimulationScreen;

        public HumanAssistedTrackSelectionScreen(
            IApplication application,
            IApplicationInstance applicationInstance)
            : base(application, applicationInstance)
        {
            trackSelectionVisual = new TrackSelectionVisual(
                application.Configuration, 
                ParentApplication, 
                "Resources\\Tracks");

            RegisterMouseClickCallback(new MouseClickCallbackEventArgs(SFML.Window.Mouse.Button.Left), trackSelectionVisual.OnMousePress);

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
                raceSimulationScreen = new HumanAssistedRacingSimulationScreen(Application, ParentApplication);

                ParentApplication.AddChildScreen(raceSimulationScreen, this);
            }
            else
            {
                ParentApplication.SetActiveScreen(raceSimulationScreen);
            }

            raceSimulationScreen.OnTrackSelected(track);
        }
    }
}
