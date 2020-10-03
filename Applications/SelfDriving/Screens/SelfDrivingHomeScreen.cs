using SelfDriving.Screens.HumanAssistedTraining;
using SelfDriving.Screens.MapMaker;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Events.CallbackArgs;
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
        }

        public override void InitializeScreen()
        {
            base.InitializeScreen();

            ConfigureGrid();

            //CreateModeScreens();
        }

        private void ConfigureGrid()
        {
            grid = new GridScreen(application.Configuration,applicationInstance);
            grid.AddColumn();
            grid.AddRow();

            var selfTrainingMenuItem = GetSelfTrainingMenuItem();
            var manualTrainingMenuItem = GetHumanTrainingMenuItem();
            var raceMenuItem = GetRaceMenuItem();
            var mapMakingMenuItem = GetMapMakingMenuItem();

            grid.AddMenuItem(0, 0, selfTrainingMenuItem);
            grid.AddMenuItem(0, 1, manualTrainingMenuItem);
            grid.AddMenuItem(1, 0, raceMenuItem);
            grid.AddMenuItem(1, 1, mapMakingMenuItem);

            RegisterMouseClickCallback(new MouseClickCallbackEventArgs(Mouse.Button.Left), grid.OnMousePress);
        }

        private void CreateModeScreens()
        {
            selfTrainingScreen = new SelfTrainingScreen(application, ParentApplication, this);
            ParentApplication.AddChildScreen(selfTrainingScreen, this);
            selfTrainingScreen.SetInactive();

            raceScreen = new RaceScreen(application, ParentApplication, this);
            ParentApplication.AddChildScreen(raceScreen, this);
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
            };

            return raceMenuItem;
        }

        private MenuItem GetHumanTrainingMenuItem()
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
                if(humanAssistedTrainingScreen == null)
                {
                    humanAssistedTrainingScreen = new HumanAssistedTrainingScreen(application, ParentApplication, this);
                    ParentApplication.AddChildScreen(humanAssistedTrainingScreen, this);
                }
                else
                {
                    ParentApplication.SetActiveScreen(humanAssistedTrainingScreen);
                }
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
                if(mapMakingScreen == null)
                {
                    mapMakingScreen = new MapMakingScreen(application, ParentApplication, this);
                    ParentApplication.AddChildScreen(mapMakingScreen, this);
                }
                else
                {
                    ParentApplication.SetActiveScreen(mapMakingScreen);
                }
            };

            return mapMakerMenuItem;
        }

        public override void OnUpdate(float deltaT)
        {
            base.OnUpdate(deltaT);

            grid.OnUpdate(deltaT);
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);

            grid.OnRender(target);
        }
    }
}
