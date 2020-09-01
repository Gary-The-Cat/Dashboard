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
        private GridVisual grid;
        private Screen activeScreen = null;
        private SelfTrainingScreen selfTrainingScreen;
        private HumanAssistedTrainingScreen humanAssistedTrainingScreen;
        private MapMakingScreen mapMakingScreen;
        private RaceScreen raceScreen;
        private IApplication application;

        public SelfDrivingHomeScreen(IApplication application) : base(application)
        {
            this.application = application;
            grid = new GridVisual(application.Window.Size, new Vector2f(0, 0));

            grid.AddColumn();
            grid.AddRow();
            grid.SetMousePressedEvent(application.Window);

            var selfTrainingMenuItem = GetSelfTrainingMenuItem();
            var manualTrainingMenuItem = GetManualTrainingMenuItem();
            var raceMenuItem = GetRaceMenuItem();
            var mapMakingMenuItem = GetMapMakingMenuItem();

            grid.AddMenuItem(0, 0, selfTrainingMenuItem);
            grid.AddMenuItem(0, 1, manualTrainingMenuItem);
            grid.AddMenuItem(1, 0, raceMenuItem);
            grid.AddMenuItem(1, 1, mapMakingMenuItem);
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
                activeScreen = raceScreen;
                grid.IsActive = false;
            };

            return raceMenuItem;
        }

        private MenuItem GetManualTrainingMenuItem()
        {
            var raceMenuItem = new MenuItem("HumanAssisted");
            var humanAssistedTexture = new Texture(new Image("Resources/HumanAssistedDriving.png"));
            humanAssistedTexture.GenerateMipmap();
            humanAssistedTexture.Smooth = true;
            raceMenuItem.Canvas = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = humanAssistedTexture,
            };
            raceMenuItem.OnClick = () =>
            {
                humanAssistedTrainingScreen = new HumanAssistedTrainingScreen(application);
                activeScreen = humanAssistedTrainingScreen;
                grid.IsActive = false;
            };

            return raceMenuItem;
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
                selfTrainingScreen = new SelfTrainingScreen(application);
                activeScreen = selfTrainingScreen;
                grid.IsActive = false;
            };

            return selfTrainingMenuItem;
        }

        private MenuItem GetMapMakingMenuItem()
        {
            var mapMakingMenuItem = new MenuItem("MapMaking");
            var raceTexture = new Texture(new Image("Resources/SelfDrivingTrackMaking.png"));
            raceTexture.GenerateMipmap();
            raceTexture.Smooth = true;
            mapMakingMenuItem.Canvas = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = raceTexture,
            };

            mapMakingMenuItem.OnClick = () =>
            {
                mapMakingScreen = new MapMakingScreen(application);
                activeScreen = mapMakingScreen;
                grid.IsActive = false;
            };

            return mapMakingMenuItem;
        }

        public override void OnUpdate(float deltaT)
        {
            activeScreen?.OnUpdate(deltaT);
        }

        public override void OnRender(RenderTarget target)
        {
            grid.OnRender(target);
            activeScreen?.OnRender(target);
        }

        public void OnExit()
        {
            grid.IsActive = false;
        }

        public void OnEnter()
        {
            if (grid != null)
            {
                grid.IsActive = true;
            }
        }
    }
}
