using SFML.Graphics;
using SFML.System;
using Shared.Interfaces;

namespace Shared.Core
{
    // This class should contain all information related to showing the application instance on the home screen. 
    // Including the layout of the visual and its position on the home screen - it's efectivel aview model
    public class ApplicationInstanceVisual
    {
        public ApplicationInstanceVisual(IApplicationInstance applicationinstance, Vector2f position)
        {
            this.ApplicationInstance = applicationinstance;
            Image.Position = position;
        }

        public IApplicationInstance ApplicationInstance { get; set; }

        public RectangleShape Image => ApplicationInstance.Thumbnail;

        public string DisplayName => ApplicationInstance.DisplayName;
    }
}
