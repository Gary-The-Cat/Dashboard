using MazeSolver.DataStructures;
using MazeSolver.MazeGeneration;
using MazeSolver.PathFinding;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using Shared.SharecdColors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeSolver.Screens
{
    public class MazeSolverScreen : Screen
    {
        private const int MazeSquareSize = 8;
        private const int MazeSquaresPerFrameToFill = 40;
        private const int SearchSpaceSquaresPerFrameToFill = 4;
        private const int SolutionSquaresPerFrameToFill = 1;

        private readonly int MazeWidth;
        private readonly int MazeHeight;

        private bool isSolvingMaze;
        private bool isDrawingExploredPaths;
        private RectangleShape[,] mazeVisuals;
        private MazeGenerator mazeGenerator;

        private int currentMazePosition = 0;
        private int mazeMapSquareCount = 0;

        private (int, int) startPosition;
        private (int, int) endPosition;

        private List<Vector2f> exploredPaths;
        private int exploredPathCount;
        private List<Vector2f> solutionPath;
        private int solutionPathCount;

        public MazeSolverScreen(
            IApplication application,
            IApplicationInstance applicationInstance) 
            :  base(application, applicationInstance)
        {
            var size = new Vector2f(application.Window.Size.X, application.Window.Size.Y);
            isSolvingMaze = false;

            MazeWidth = (int)(size.X / MazeSquareSize);
            MazeHeight = (int)(size.Y / MazeSquareSize);

            // Passing this in backwards for simplicity?
            mazeGenerator = new MazeGenerator(MazeWidth, MazeHeight);
            this.GenerateNewSolvedMaze();

            mazeVisuals = this.GetMazeVisuals();
        }

        public override void OnUpdate(float dt)
        {
            if (isSolvingMaze)
            {
                this.FillMazeSolutionSquare();
            }
            else
            {
                this.FillMazePathSquare();
            }
        }

        public override void OnRender(RenderTarget target)
        {
            foreach(var visual in mazeVisuals)
            {
                target.Draw(visual);
            }
        }

        private void FillMazeSolutionSquare()
        {
            if (isDrawingExploredPaths)
            {
                for (int i = 0; i < SearchSpaceSquaresPerFrameToFill; i++)
                {
                    // Drawing the paths explored by the A* algorithm

                    if (currentMazePosition < exploredPathCount)
                    {
                        var point = exploredPaths[currentMazePosition++];
                        var color = new HslColor((point.X / MazeWidth) * 255, 180, 130);
                        mazeVisuals[(int)point.X, (int)point.Y].FillColor = color;
                    }
                    else
                    {
                        isDrawingExploredPaths = false;
                        currentMazePosition = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < SolutionSquaresPerFrameToFill; i++)
                {
                    // Drawing the final path
                    if (currentMazePosition < solutionPathCount)
                    {
                        var point = solutionPath[currentMazePosition++];
                        var color = new HslColor((point.X / MazeWidth) * 255, 255, 190);
                        mazeVisuals[(int)point.X, (int)point.Y].FillColor = color;
                    }
                    else
                    {
                        isSolvingMaze = false;
                        this.ClearMazeVisuals();
                        this.GenerateNewSolvedMaze();
                        currentMazePosition = 0;
                    }
                }
            }
        }

        private void FillMazePathSquare()
        {
            for (int i = 0; i < MazeSquaresPerFrameToFill; i++)
            {
                if (currentMazePosition < mazeMapSquareCount)
                {
                    // We are drawing the maze, get the next square, get its position, ans turn it white
                    var cell = mazeGenerator.mapGeneration[currentMazePosition];
                    int x = cell.row;
                    int y = cell.col;

                    mazeVisuals[x, y].FillColor = Color.White;

                    // Increment our position for the next time through the loop
                    currentMazePosition++;
                }
                else
                {
                    // We have finished drawing the maze, toggle solve mode
                    isSolvingMaze = true;
                    isDrawingExploredPaths = true;
                    currentMazePosition = 0;

                    // Mark the starting and end positions
                    mazeVisuals[startPosition.Item1, startPosition.Item2].FillColor = Color.Red;
                    mazeVisuals[endPosition.Item1, endPosition.Item2].FillColor = Color.Green;

                    // We don't need to attempt filling the remaining squares.
                    break;
                }
            }
        }

        private RectangleShape[,] GetMazeVisuals()
        {
            RectangleShape[,] visuals = new RectangleShape[MazeWidth, MazeHeight];
            var shapeSize = new Vector2f(MazeSquareSize, MazeSquareSize);

            for (int y = 0; y < MazeHeight; y++)
            {
                for (int x = 0; x < MazeWidth; x++)
                {
                    var mazeSquare = new RectangleShape(shapeSize);
                    mazeSquare.Position = new Vector2f(x * MazeSquareSize, y * MazeSquareSize);
                    mazeSquare.FillColor = new Color(0x2e, 0x2e, 0x2e);
                    visuals[x, y] = mazeSquare;
                }
            }

            return visuals;
        }

        private void ClearMazeVisuals()
        {
            for (int y = 0; y < MazeHeight; y++)
            {
                for (int x = 0; x < MazeWidth; x++)
                {
                    mazeVisuals[x, y].FillColor = new Color(0x2e, 0x2e, 0x2e);                    
                }
            }
        }

        private void GenerateNewSolvedMaze()
        {
            mazeGenerator.Generate();
            mazeMapSquareCount = mazeGenerator.mapGeneration.Count();

            this.startPosition = GetViableStartEndPosition(mazeGenerator.mapGeneration);
            this.endPosition = GetViableStartEndPosition(mazeGenerator.mapGeneration);

            (solutionPath, exploredPaths) = this.SolveMaze();

            solutionPathCount = solutionPath.Count;
            exploredPathCount = exploredPaths.Count;
        }

        private (int, int) GetViableStartEndPosition(List<Cell> mapGeneration)
        {
            Random random = new Random();
            var position = random.Next(mapGeneration.Count);
            var mapCell = mapGeneration[position];

            return (mapCell.row, mapCell.col);
        }

        private (List<Vector2f> path, List<Vector2f> exploredPaths) SolveMaze()
        {
            Maze mazePuzzle = new Maze(mazeGenerator.GetMazeList(), startPosition, endPosition);

            AStar solver = new AStar(MazeWidth, MazeHeight);
            solver.Solve(mazePuzzle);
            var exploredPaths = solver.GetExploredPaths();
            var solution = solver.GetResult();

            return (solution, exploredPaths);
        }
    }
}
