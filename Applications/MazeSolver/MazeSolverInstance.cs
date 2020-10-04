using MazeSolver.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace MazeSolver
{
    public class MazeSolverInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public MazeSolverInstance(IApplication application) : base(application)
        {
            var texture = new Texture(new Image("Resources\\MazeSolver.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Maze Solver";

        public new void Initialize()
        {
            AddChildScreen(new MazeSolverScreen(Application, this), null);

            base.Initialize();
        }
    }
}
