using SFML.System;
using System;
using System.Collections.Generic;

namespace MazeSolver.DataStructures
{
    public class Maze
    {

        public List<List<bool>> MazePuzzle;

        public (int X, int Y) InitialPosition;

        public (int X, int Y) GoalPosition;

        public Maze(List<List<bool>> maze, (int, int) initialPosition, (int, int) goalPosition)
        {
            MazePuzzle = maze;
            InitialPosition = initialPosition;
            GoalPosition = goalPosition;
        }

        public double[,] GetPuzzle(int mazeWidth, int mazeHeight)
        {
            double[,] HeuristicMap = new double[mazeWidth, mazeHeight];

            for (int x = 0; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    HeuristicMap[x, y] = Math.Sqrt(((x - GoalPosition.X) * (x - GoalPosition.X)) + ((y - GoalPosition.Y) * (y - GoalPosition.Y)));
                }
            }

            return HeuristicMap;
        }
    }
}
