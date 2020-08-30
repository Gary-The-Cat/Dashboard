using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakingScreen : Screen
    {
        private IApplication application;
        private MapMakerState state;

        private StackedScreen mapEditorScreen;
        private TrackSelectionVisual trackSelection;

        public MapMakingScreen(IApplication application)
        {
            this.application = application;
            this.state = MapMakerState.TrackSelection;

            trackSelection = new TrackSelectionVisual(
                application,
                new Vector2f(0, 0),
                "Resources/Tracks");

            trackSelection.OnTrackSelected = OnTrackSelected;

            mapEditorScreen = new StackedScreen();

            mapEditorScreen.AddScreen(new MapMakerHudScreen(application));

            trackSelection.InsertTrack(new Track(), 0);
        }

        public override void OnUpdate(float dt)
        {
            switch (state)
            {
                case MapMakerState.TrackEditor:
                    mapEditorScreen.OnUpdate(dt);
                    break;
                case MapMakerState.TrackSelection:
                    break;
            }
        }

        public override void OnRender(RenderTarget target)
        {
            switch (state)
            {
                case MapMakerState.TrackEditor:
                    mapEditorScreen.OnRender(target);
                    break;
                case MapMakerState.TrackSelection:
                    trackSelection.OnRender(target);
                    break;
            }
        }

        private void OnTrackSelected(Track obj)
        {
            state = MapMakerState.TrackEditor;
        }
    }
}
