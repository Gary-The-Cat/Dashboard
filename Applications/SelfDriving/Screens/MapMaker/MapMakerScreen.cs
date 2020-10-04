using SelfDriving.Shared;
using Shared.Core.Hierarchy;
using Shared.Interfaces;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakerScreen : StackedScreen
    {
        private MapMakerHudScreen mapEditorHudScreen;
        private MapMakerWorldScreen mapEditorWorldScreen;

        public MapMakerScreen(
            IApplication application,
            IApplicationInstance applicationInstance)
            : base(application, applicationInstance)
        {
            var sharedContainer = new MapMakerDataContainer();

            mapEditorWorldScreen = new MapMakerWorldScreen(Application, ParentApplication, sharedContainer);
            mapEditorHudScreen = new MapMakerHudScreen(Application, ParentApplication, sharedContainer);

            AddScreen(mapEditorWorldScreen);
            AddScreen(mapEditorHudScreen);
        }

        public void Initialize(Track track)
        {
            mapEditorWorldScreen.Initialize(track);
        }
    }
}
