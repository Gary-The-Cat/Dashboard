using SelfDriving.Screens.TrackSelection;
using SelfDriving.Shared;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakingScreen : Screen
    {
        private IApplication application;
        private IApplicationInstance applicationInstance;
        private MapMakerState state;
        private MapMakerHudScreen mapEditorHudScreen;
        private MapMakerWorldScreen mapEditorWorldScreen;
        private Screen parentScreen;

        private TrackSelectionScreen trackSelection;

        public MapMakingScreen(
            IApplication application,
            IApplicationInstance applicationInstance,
            Screen parentScreen) 
            : base(application.Configuration, applicationInstance)
        {
            this.application = application;
            this.applicationInstance = applicationInstance;
            this.parentScreen = parentScreen;
            this.state = MapMakerState.TrackSelection;

            trackSelection = new TrackSelectionScreen(
                application.Configuration,
                applicationInstance,
                "Resources/Tracks");

            trackSelection.OnTrackSelected = OnTrackSelected;

            trackSelection.InsertTrack(new Track(), 0);
        }

        public override void OnRender(RenderTarget target)
        {
            trackSelection.OnRender(target);
        }

        private void OnTrackSelected(Track track)
        {
            SetInactive();

            this.state = MapMakerState.TrackEditor;

            var sharedContainer = new MapMakerDataContainer();

            mapEditorWorldScreen = new MapMakerWorldScreen(application, applicationInstance, sharedContainer);
            mapEditorHudScreen = new MapMakerHudScreen(application, applicationInstance, sharedContainer);

            applicationInstance.AddScreen(mapEditorHudScreen);
            applicationInstance.AddScreen(mapEditorWorldScreen);

            mapEditorWorldScreen.Start();
            mapEditorHudScreen.Start();

            mapEditorWorldScreen.Initialize(track);
        }

        public override void SetActive()
        {
            base.SetActive();

            this.applicationInstance.GoBack = () =>
            {
                if(this.state == MapMakerState.TrackSelection)
                {
                    this.SetInactive();
                    this.parentScreen.SetActive();
                }
                else
                {
                    this.state = MapMakerState.TrackSelection;

                    mapEditorWorldScreen.SetInactive();
                    mapEditorHudScreen.SetInactive();
                    trackSelection.SetActive();
                }
            };

            trackSelection.SetActive();
        }

        public override void SetInactive()
        {
            base.SetInactive();

            trackSelection.SetInactive();
        }
    }
}
