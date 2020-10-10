using Ninject;
using Ninject.Parameters;
using SelfDriving.Shared;
using Shared.Core.Hierarchy;
using Shared.Interfaces.Services;

namespace SelfDriving.Screens.MapMaker
{
    public class MapMakerScreen : StackedScreen
    {
        private MapMakerHudScreen mapEditorHudScreen;
        private MapMakerWorldScreen mapEditorWorldScreen;

        public MapMakerScreen(IApplicationService appService)
        {
            var sharedContainer = new MapMakerDataContainer();

            mapEditorWorldScreen = appService.Kernel.Get<MapMakerWorldScreen>(
                new ConstructorArgument("sharedContainer", sharedContainer));

            mapEditorHudScreen = appService.Kernel.Get<MapMakerHudScreen>(
                new ConstructorArgument("sharedContainer", sharedContainer));

            AddScreen(mapEditorWorldScreen);
            AddScreen(mapEditorHudScreen);
        }

        public void Initialize(Track track)
        {
            mapEditorWorldScreen.Initialize(track);
        }
    }
}
