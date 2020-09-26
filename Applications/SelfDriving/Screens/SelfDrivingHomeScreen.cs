using SelfDriving.Screens.HumanAssistedTraining;
using SelfDriving.Screens.MapMaker;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using Shared.Menus;

namespace SelfDriving.Screens
{
    public class SelfDrivingHomeScreen : Screen
    {
        private GridScreen grid;
        private SelfTrainingScreen selfTrainingScreen;
        private HumanAssistedTrainingScreen humanAssistedTrainingScreen;
        private MapMakingScreen mapMakingScreen;
        private RaceScreen raceScreen;
        private IApplication application;
        private IApplicationInstance applicationInstance;

        public SelfDrivingHomeScreen(
            IApplication application,
            IApplicationInstance applicationInstance) 
            : base(application.Configuration, applicationInstance)
        {
            this.application = application;
            this.applicationInstance = applicationInstance;

            ConfigureGrid();

            applicationInstance.AddScreen(grid);

            CreateModeScreens();
        }

        private void ConfigureGrid()
        {
            grid = new GridScreen(application.Configuration,applicationInstance);
            grid.AddColumn();
            grid.AddRow();

            var selfTrainingMenuItem = GetSelfTrainingMenuItem();
            var manualTrainingMenuItem = GetManualTrainingMenuItem();
            var raceMenuItem = GetRaceMenuItem();
            var mapMakingMenuItem = GetMapMakingMenuItem();

            grid.AddMenuItem(0, 0, selfTrainingMenuItem);
            grid.AddMenuItem(0, 1, manualTrainingMenuItem);
            grid.AddMenuItem(1, 0, raceMenuItem);
            grid.AddMenuItem(1, 1, mapMakingMenuItem);

            AddChildScreen(grid);
        }

        private void CreateModeScreens()
        {
            humanAssistedTrainingScreen = new HumanAssistedTrainingScreen(application, ParentApplication);
            ParentApplication.AddScreen(humanAssistedTrainingScreen);
            humanAssistedTrainingScreen.SetInactive();

            mapMakingScreen = new MapMakingScreen(application, ParentApplication);
            ParentApplication.AddScreen(mapMakingScreen);
            mapMakingScreen.SetInactive();

            selfTrainingScreen = new SelfTrainingScreen(application, ParentApplication);
            ParentApplication.AddScreen(selfTrainingScreen);
            selfTrainingScreen.SetInactive();

            raceScreen = new RaceScreen(application, ParentApplication);
            ParentApplication.AddScreen(raceScreen);
            raceScreen.SetInactive();
        }

        private MenuItem GetRaceMenuItem()
        {
            var raceMenuItem = new MenuItem("RaceMenu");
            var raceTexture = new Texture(new Image("Resources/SelfDrivingRace.png"));
            raceTexture.GenerateMipmap();
            raceTexture.Smooth = true;
            raceMenuItem.Canvas = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = raceTexture,
            };
            raceMenuItem.OnClick = () =>
            {
                SetActiveScreen(raceScreen);
            };

            return raceMenuItem;
        }

        private MenuItem GetManualTrainingMenuItem()
        {
            var humanAssistedMenuItem = new MenuItem("HumanAssisted");
            var humanAssistedTexture = new Texture(new Image("Resources/HumanAssistedDriving.png"));
            humanAssistedTexture.GenerateMipmap();
            humanAssistedTexture.Smooth = true;
            humanAssistedMenuItem.Canvas = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = humanAssistedTexture,
            };
            humanAssistedMenuItem.OnClick = () =>
            {
                SetActiveScreen(humanAssistedTrainingScreen);
            };

            return humanAssistedMenuItem;
        }

        private MenuItem GetSelfTrainingMenuItem()
        {
            var selfTrainingMenuItem = new MenuItem("SelfTraining");
            var selfTrainingTexture = new Texture(new Image("Resources/SelfTraining.png"));
            selfTrainingTexture.GenerateMipmap();
            selfTrainingTexture.Smooth = true;

            selfTrainingMenuItem.Canvas = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = selfTrainingTexture,
            };

            selfTrainingMenuItem.OnClick = () =>
            {
                SetActiveScreen(selfTrainingScreen);
            };

            return selfTrainingMenuItem;
        }

        private MenuItem GetMapMakingMenuItem()
        {
            var mapMakerMenuItem = new MenuItem("MapMaking");
            var raceTexture = new Texture(new Image("Resources/SelfDrivingTrackMaking.png"));
            raceTexture.GenerateMipmap();
            raceTexture.Smooth = true;
            mapMakerMenuItem.Canvas = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = raceTexture,
            };

            mapMakerMenuItem.OnClick = () =>
            {
                SetActiveScreen(mapMakingScreen);
            };

            return mapMakerMenuItem;
        }

        public void SetActiveScreen(Screen screen)
        {
            SetInactive();
            screen.SetActive();
        }
    }
}
