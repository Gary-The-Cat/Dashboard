using Newtonsoft.Json;
using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.Interfaces;
using Shared.Menus;
using Shared.Notifications;
using System;
using System.Collections.Generic;
using System.IO;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakerHudScreen : Screen
    {
        private IApplication application;

        private List<Button> buttons;

        private MapMakerDataContainer sharedContainer;

        public MapMakerHudScreen(
            IApplication application, 
            IApplicationInstance applicationInstance,
            MapMakerDataContainer sharedContainer) 
            : base(application, applicationInstance)
        {
            this.application = application;

            this.sharedContainer = sharedContainer;

            this.buttons = new List<Button>();

            RegisterMouseClickCallback(
                new MouseClickCallbackEventArgs(Mouse.Button.Left),
                OnMousePress);

            buttons.Add(new Button("Draw", new Vector2f(20, 20), () =>
            {
                SetState(MapEditState.DrawingLines);
                application.NotificaitonService.ShowToast(
                    ToastType.Info,
                    "Drawing Lines Enabled");
            }, HorizontalAlignment.Left));

            buttons.Add(new Button("Move", new Vector2f(20, 70), () => 
            {
                SetState(MapEditState.MovingPoints);
                application.NotificaitonService.ShowToast(
                    ToastType.Info,
                    "Moving Points Enabled");
            }, HorizontalAlignment.Left));

            buttons.Add(new Button("Delete", new Vector2f(20, 120), () => 
            {
                SetState(MapEditState.Deletion);
                application.NotificaitonService.ShowToast(
                    ToastType.Warning,
                    "Deletion Enabled");
            }, HorizontalAlignment.Left));

            buttons.Add(new Button("Checkpoints", new Vector2f(20, 170), () =>
            {
                SetState(MapEditState.Checkpoint);
                application.NotificaitonService.ShowToast(
                    ToastType.Info,
                    "Checkpoint Mode Enabled");
            }, HorizontalAlignment.Left));

            buttons.Add(new Button("Start", new Vector2f(20, 225), () =>
            {
                SetState(MapEditState.StartPosition);
                application.NotificaitonService.ShowToast(
                    ToastType.Info,
                    "Set Start Position");
            }, HorizontalAlignment.Left));

            var exportTextPosition = new Vector2f(20, application.Window.Size.Y - 60);
            buttons.Add(new Button("Export", exportTextPosition, () => ExportTrack(), HorizontalAlignment.Left));
        }

        private void ExportTrack()
        {
            var track = new Track();

            track.Map = sharedContainer.GetMap();
            track.Checkpoints = sharedContainer.GetCheckpoints();
            track.StartPosition = sharedContainer.StartPosition;
            track.InitialHeading = sharedContainer.StartRotation;

            var trackText = JsonConvert.SerializeObject(track, Formatting.Indented);

            var baseFileName = $"{Directory.GetCurrentDirectory()}\\Track_{DateTime.Now:YY_DD_MM_hh.mm.ss}";
            var trackFileName = $"{baseFileName}.json";
            var trackThumbnailName = $"{baseFileName}.png";
            File.WriteAllText(trackFileName, trackText);

            ThumbnailHelper.GenerateTrackThumbnail(track, trackThumbnailName);

            application.NotificaitonService.ShowToast(
                ToastType.Successful,
                "Track exported successfully");
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
        }

        public override void OnRender(RenderTarget target)
        {
            target.SetView(application.GetDefaultView());

            buttons.ForEach(b => b.OnRender(target));
        }

        private void SetState(MapEditState state)
        {
            this.sharedContainer.EditState = state;
        }

        private void OnMousePress(MouseClickEventArgs args)
        {
            buttons.ForEach(b => b.TryClick(args));
        }
    }
}
