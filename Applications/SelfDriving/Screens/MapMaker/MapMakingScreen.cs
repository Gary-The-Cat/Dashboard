﻿using SelfDriving.Shared;
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
        private MapMakerHudScreen mapEditorHudScreen;
        private MapMakerWorldScreen mapEditorWorldScreen;

        private TrackSelectionVisual trackSelection;

        public MapMakingScreen(
            IApplication application,
            IApplicationInstance applicationInstance) 
            : base(application.Configuration, applicationInstance)
        {
            this.application = application;
            this.state = MapMakerState.TrackSelection;

            var sharedContainer = new MapMakerDataContainer();

            trackSelection = new TrackSelectionVisual(
                application,
                new Vector2f(0, 0),
                "Resources/Tracks");

            trackSelection.OnTrackSelected = OnTrackSelected;

            mapEditorWorldScreen = new MapMakerWorldScreen(application, applicationInstance, sharedContainer);
            mapEditorWorldScreen.SetInactive();

            mapEditorHudScreen = new MapMakerHudScreen(application, applicationInstance, sharedContainer);
            mapEditorHudScreen.SetInactive();

            applicationInstance.AddScreen(mapEditorHudScreen);
            applicationInstance.AddScreen(mapEditorWorldScreen);

            trackSelection.InsertTrack(new Track(), 0);
        }

        public override void OnRender(RenderTarget target)
        {
            trackSelection.OnRender(target);
        }

        private void OnTrackSelected(Track track)
        {
            SetInactive();

            mapEditorWorldScreen.SetActive();
            mapEditorHudScreen.SetActive();

            mapEditorWorldScreen.Initialize(track);
        }
    }
}
