﻿using SelfDriving.Helpers;
using SFML.Graphics;
using SFML.System;
using Shared.Interfaces;
using Shared.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SelfDriving.Shared
{
    public class TrackSelectionVisual
    {
        private List<Track> tracks;

        private GridVisual grid;

        public Action<Track> OnTrackSelected;

        public TrackSelectionVisual(
            IApplication application,
            Vector2f position,
            string trackDirectory)
        {
            grid = new GridVisual(application.Window.Size, position);

            grid.SetMousePressedEvent(application.Window);

            tracks = TrackHelper.LoadTrackFiles(trackDirectory);

            PopulateTrackVisuals();
        }

        public void InsertTrack(Track track, int index)
        {
            tracks.Insert(index, track);

            PopulateTrackVisuals();
        }

        private void PopulateTrackVisuals()
        {
            var trackVisuals = new List<MenuItem>();
            grid.Clear();

            foreach (var track in tracks)
            {
                // We couldnt find the matching image for this track, don't add it.
                var texture = GetTrackTexture(track.FileLocation);

                MenuItem trackVisual = null;

                if (texture == null)
                {
                    trackVisual = GetDefaultTrackVisual(track);
                }
                else
                {
                    trackVisual = GetTrackVisual(track, texture);
                }

                trackVisuals.Add(trackVisual);
            }

            if (trackVisuals.Any())
            {
                for (int i = 0; i < trackVisuals.Count - 1; i++)
                {
                    grid.AddColumn();
                    grid.AddMenuItem(0, i, trackVisuals[i]);
                }

                grid.AddMenuItem(0, trackVisuals.Count - 1, trackVisuals.Last());
            }
        }

        private MenuItem GetTrackVisual(Track track, Texture texture)
        {
            var trackVisual = new MenuItem(track.FileLocation);
            trackVisual.Canvas = new RectangleShape()
            {
                Texture = texture,
            };

            trackVisual.OnClick = () =>
            {
                this.OnTrackSelected?.Invoke(track);
                grid.IsActive = false;
            };

            return trackVisual;
        }

        private MenuItem GetDefaultTrackVisual(Track track)
        {
            var trackVisual = new MenuItem(track.FileLocation);
            trackVisual.Canvas = new RectangleShape()
            {
                FillColor = new Color(32, 126, 160),
            };

            trackVisual.OnClick = () =>
            {
                this.OnTrackSelected?.Invoke(track);
                grid.IsActive = false;
            };

            return trackVisual;
        }

        private Texture GetTrackTexture(string fileLocation)
        {
            try
            {
                var fileSansExtension = Path.GetFileNameWithoutExtension(fileLocation);
                var filePath = Path.GetDirectoryName(fileLocation);
                var expectedImageLocation = Path.Combine(filePath, fileSansExtension + ".png");
                var image = new Image(expectedImageLocation);
                var texture = new Texture(image);
                return texture;
            }
            catch
            {
                return null;
            }
        }

        public void SetActive(bool isActive)
        {
            grid.IsActive = isActive;
        }

        public void OnRender(RenderTarget target)
        {
            grid.OnRender(target);
        }
    }
}
