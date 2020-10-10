using MazeSolver.Screens;
using Ninject;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using Shared.Interfaces.Services;

namespace MazeSolver
{
    public class MazeSolverInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private IApplicationService appService;

        public MazeSolverInstance(IApplicationService appService)
        {
            this.appService = appService;

            var texture = new Texture(new Image("Resources\\MazeSolver.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Maze Solver";

        public new void Initialize()
        {
            var mazeSolverScreen = appService.Kernel.Get<MazeSolverScreen>();
            AddChildScreen(mazeSolverScreen);

            base.Initialize();
        }
    }
}
