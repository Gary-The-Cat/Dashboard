using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Core.Hierarchy
{
    public class ScreenManager
    {
        private Dictionary<int, Layer> layers;

        private Dictionary<Screen, int> screenToLayerLookup;

        private Dictionary<Guid, Screen> screenLookup;

        public int CurrentLayer { get; private set; }

        public Screen ActiveScreen => layers[CurrentLayer].ActiveScreen;

        public ScreenManager()
        {
            layers = new Dictionary<int, Layer>();

            screenLookup = new Dictionary<Guid, Screen>();

            screenToLayerLookup = new Dictionary<Screen, int>();
        }

        internal void GoBack()
        {
            CurrentLayer--;
        }

        public void AddChildScreen(Screen screen, Screen parentScreen)
        {
            int layerId = 0;

            Screen adjustedParentScreen = parentScreen;

            if (parentScreen != null)
            {
                if (parentScreen.IsStackedChild)
                {
                    adjustedParentScreen = screenLookup[parentScreen.StackedParentId];
                }

                if (!screenToLayerLookup.ContainsKey(adjustedParentScreen))
                {
                    throw new Exception("The parent screen cannot be found. Please ensure the parent screen has been added to the application before adding children.");
                }

                layerId = screenToLayerLookup[adjustedParentScreen] + 1;
            }

            if (!layers.ContainsKey(layerId))
            {
                layers.Add(layerId, new Layer(layerId));
            }

            screenLookup.Add(screen.Id, screen);
            screenToLayerLookup.Add(screen, layerId);
            layers[layerId].AddScreen(screen);
            screen.InitializeScreen();
            SetActiveScreen(screen);
        }

        public void SetActiveScreen(Screen screen)
        {
            if (!screenToLayerLookup.ContainsKey(screen))
            {
                throw new ArgumentException("The screen provided does not exist. Please add it using the 'AddChildScreen' command before setting it active.");
            }

            var layerId = screenToLayerLookup[screen];
            CurrentLayer = layerId;

            layers[CurrentLayer].SetActiveScreen(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            layers[screenToLayerLookup[screen]].RemoveScreen(screen);

            // :TODO: If there are no screens left on this layer, do we:
            // - remove all lower layers? 
            // - shift them up one layer?
            throw new NotImplementedException();
        }

        public void OnResize(float width, float height)
        {
            foreach (var kvp in layers)
            {
                foreach (var screen in kvp.Value.Screens)
                {
                    screen.Camera.ScaleToWindow(width, height);
                }
            }
        }

        public void OnUpdate(float deltaT)
        {
            ActiveScreen.OnUpdate(deltaT);
        }

        public void OnRender(RenderTarget target)
        {
            ActiveScreen.OnRender(target);
        }

        public void Start()
        {
            ActiveScreen.Start();
        }
    }
}
