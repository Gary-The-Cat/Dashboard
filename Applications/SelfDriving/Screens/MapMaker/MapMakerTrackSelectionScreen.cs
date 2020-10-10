using Ninject;
using Ninject.Parameters;
using SelfDriving.Screens.TrackSelection;
using SelfDriving.Shared;
using SFML.Graphics;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Interfaces;
using Shared.Interfaces.Services;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakerTrackSelectionScreen : Screen
    {
        private MapMakerScreen mapMakerScreen;

        private TrackSelectionVisual trackSelectionVisual;

        private IApplicationService appService;

        private IApplicationManager appManager;

        public MapMakerTrackSelectionScreen(
            IApplicationService appService,
            IApplicationManager appManager,
            IEventService eventService)
        {
            this.appService = appService;
            this.appManager = appManager;

            trackSelectionVisual = appService.Kernel.Get<TrackSelectionVisual>(
                new ConstructorArgument("trackDirectory", "Resources\\Tracks"));

            eventService.RegisterMouseClickCallback(
                this.Id, 
                new MouseClickCallbackEventArgs(SFML.Window.Mouse.Button.Left), 
                trackSelectionVisual.OnMousePress);

            trackSelectionVisual.OnTrackSelected = OnTrackSelected;

            trackSelectionVisual.InsertTrack(new Track(), 0);
        }

        public override void OnRender(RenderTarget target)
        {
            trackSelectionVisual.OnRender(target);
        }

        private void OnTrackSelected(Track track)
        {
            if(mapMakerScreen == null)
            {
                mapMakerScreen = appService.Kernel.Get<MapMakerScreen>();
                appManager.AddChildScreen(mapMakerScreen);
            }
            else
            {
                appManager.SetActiveScreen(mapMakerScreen);
            }

            mapMakerScreen.Initialize(track);
        }
    }
}
