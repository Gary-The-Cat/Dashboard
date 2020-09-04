using MazeSolver.Screens;
using SFML.Graphics;
using Shared.Core;
using Shared.Interfaces;
using System;

namespace MazeSolver
{
    public class MazeSolverInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public MazeSolverInstance(IApplication application)
        {
            this.Application = application;

            var texture = new Texture(new Image("Resources\\MazeSolver.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new SFML.System.Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public IApplication Application { get; set; }

        public string DisplayName => "Maze Solver";

        public RectangleShape Thumbnail { get; set; }

        public override Screen Screen { get; set; }

        public RenderWindow RenderWindow { get; set; }

        public new void Initialize()
        {
            base.Initialize();

            Screen = new MazeSolverScreen(Application);
            Application.ApplicationManager.AddScreen(Screen);
        }

        public new void Start()
        {
            base.Start();
        }
    }
}
