using Newtonsoft.Json;
using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Interfaces;
using Shared.Menus;
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
        Button stateText;

        public MapMakerHudScreen(
            IApplication application, 
            IApplicationInstance applicationInstance,
            MapMakerDataContainer sharedContainer) 
            : base(application.Configuration, applicationInstance)
        {
            this.application = application;

            this.sharedContainer = sharedContainer;

            this.buttons = new List<Button>();

            RegisterMouseClickCallback(
                application.Window,
                Mouse.Button.Right,
                OnMousePress);

            var stateTextPosition = new Vector2f(application.Window.Size.X - 120, application.Window.Size.Y - 60);

            stateText = new Button(
                MapEditState.DrawingLines.ToString(), 
                stateTextPosition,
                () => { },
                HorizontalAlignment.Centre);

            buttons.Add(new Button("Draw", new Vector2f(20, 20), () => SetState(MapEditState.DrawingLines), HorizontalAlignment.Left));
            buttons.Add(new Button("Move", new Vector2f(20, 70), () => SetState(MapEditState.MovingPoints), HorizontalAlignment.Left));
            buttons.Add(new Button("Delete", new Vector2f(20, 120), () => SetState(MapEditState.Deletion), HorizontalAlignment.Left));
            buttons.Add(new Button("Checkpoints", new Vector2f(20, 170), () => SetState(MapEditState.Checkpoint), HorizontalAlignment.Left));
            buttons.Add(new Button("Start", new Vector2f(20, 225), () => SetState(MapEditState.StartPosition), HorizontalAlignment.Left));

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

            var saveName = $"{Directory.GetCurrentDirectory()}\\Track_{DateTime.Now:YY_DD_MM_hh.mm.ss}.json";
            File.WriteAllText(saveName, trackText);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            buttons.ForEach(b => b.OnUpdate());
        }

        public override void OnRender(RenderTarget target)
        {
            target.SetView(application.GetDefaultView());

            buttons.ForEach(b => b.OnRender(target));

            stateText.OnRender(target);
        }

        private void SetState(MapEditState state)
        {
            this.sharedContainer.EditState = state;
            this.stateText.Text = state.ToString();
        }

        private void OnMousePress(float x, float y)
        {
            buttons.ForEach(b => 
            {
                if (b.GetGlobalBounds().Contains(x, y))
                {
                    b.OnClick();
                }
            });
        }
    }
}
