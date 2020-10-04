using SelfDriving.Screens.TrackSelection;
using SelfDriving.Shared;
using SFML.Graphics;
using Shared.Core;
using Shared.Core.Hierarchy;
using Shared.Events.CallbackArgs;
using Shared.Interfaces;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakerTrackSelectionScreen : Screen
    {
        private MapMakerScreen mapMakerScreen;

        private TrackSelectionVisual trackSelectionVisual;

        public MapMakerTrackSelectionScreen(
            IApplication application,
            IApplicationInstance applicationInstance) 
            : base(application, applicationInstance)
        {
            trackSelectionVisual = new TrackSelectionVisual(
                application.Configuration,
                applicationInstance,
                "Resources/Tracks");

            RegisterMouseClickCallback(new MouseClickCallbackEventArgs(SFML.Window.Mouse.Button.Left), trackSelectionVisual.OnMousePress);

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
                mapMakerScreen = new MapMakerScreen(Application, ParentApplication);
                ParentApplication.AddChildScreen(mapMakerScreen, this);
            }
            else
            {
                ParentApplication.SetActiveScreen(mapMakerScreen);
            }

            mapMakerScreen.Initialize(track);
        }
    }
}
