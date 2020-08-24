using MazeSolver.DataStructures;
using MazeSolver.Interfaces;
using SFML.System;
using System;
using System.Collections.Generic;

namespace MazeSolver.PathFinding
{
    class AStar : IMazeSolver
    {

        private Node currentPosition;

        private Vector2f goalPosiiton;

        private List<Node> frontier;

        private List<Vector2f> resultPath;

        private List<Vector2f> exploredPaths;

        private bool[,] closedNodes;

        private bool[,] frontierMap;

        private readonly int MazeWidth;

        private readonly int MazeHeight;

        public AStar(int mazeWidth, int mazeHeight)
        {
            MazeWidth = mazeWidth;
            MazeHeight = mazeHeight;

            closedNodes = new bool[MazeWidth, MazeHeight];
            frontierMap = new bool[MazeWidth, MazeHeight];
            frontier = new List<Node>();
            resultPath = new List<Vector2f>();
            exploredPaths = new List<Vector2f>();
        }

        public List<Vector2f> GetResult()
        {
            return resultPath;
        }

        public List<Vector2f> GetExploredPaths()
        {
            return exploredPaths;
        }

        public void Solve(Maze maze)
        {
            currentPosition = new Node(
                new Vector2f(maze.InitialPosition.X, maze.InitialPosition.Y), 
                0, 
                GetDistance(
                    new Vector2f(maze.InitialPosition.X, maze.InitialPosition.Y), 
                    new Vector2f(maze.GoalPosition.X, maze.GoalPosition.Y)), 
                null);

            goalPosiiton = new Vector2f(maze.GoalPosition.X, maze.GoalPosition.Y);
            FillClosedNodes(maze);

            //While we havent reached the goal
            while (currentPosition.Position.X != goalPosiiton.X || currentPosition.Position.Y != goalPosiiton.Y)
            {
                //Close current node
                closedNodes[(int)currentPosition.Position.X, (int)currentPosition.Position.Y] = true;

                // Expand all nodes
                AddNeighbourNodes();

                // Check frontier for cheapest node
                currentPosition = GetBestFrontierNode();

                exploredPaths.Add(new Vector2f(currentPosition.Position.X, currentPosition.Position.Y));
            }

            while (currentPosition != null)
            {
                resultPath.Add(new Vector2f(currentPosition.Position.X, currentPosition.Position.Y));
                currentPosition = currentPosition.Parent;
            }
        }

        private void FillClosedNodes(Maze maze)
        {
            for (int x = 0; x < MazeWidth; x++)
            {
                for (int y = 0; y < MazeHeight; y++)
                {
                    closedNodes[x, y] = maze.MazePuzzle[x][y];
                    if (maze.MazePuzzle[x][y]) Console.Write("1 ");
                    else Console.Write("0 ");
                }
                Console.WriteLine();
            }
        }

        private Node GetBestFrontierNode()
        {
            Node node = frontier[0];
            foreach (Node n in frontier)
            {
                if (n.TotalCost < node.TotalCost)
                {
                    node = n;
                }
            }
            frontier.Remove(node);

            return node;
        }

        private void AddNeighbourNodes()
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // Only check manhattan
                    if (x * y != 0)
                    {
                        continue;
                    }

                    if (CheckBounds(new Vector2f(currentPosition.Position.X + x, currentPosition.Position.Y + y)))
                    {
                        if (!closedNodes[(int)currentPosition.Position.X + x, (int)currentPosition.Position.Y + y])
                        {
                            if (!frontierMap[(int)currentPosition.Position.X + x, (int)currentPosition.Position.Y + y])
                            {
                                var newPosition = new Vector2f(currentPosition.Position.X + x, currentPosition.Position.Y + y);
                                var newNode = new Node(
                                    newPosition,
                                    currentPosition.PathCost + GetDistance(newPosition, currentPosition.Position),
                                    GetDistance(newPosition, goalPosiiton), currentPosition.LazyClone());
                                frontier.Add(newNode);
                                frontierMap[(int)currentPosition.Position.X + x, (int)currentPosition.Position.Y + y] = true;
                            }
                        }
                    }
                }
            }
        }

        private bool CheckBounds(Vector2f point)
        {
            if (point.X < 0 || point.Y < 0 || point.X >= MazeWidth || point.Y >= MazeHeight)
            {
                return false;
            }
            return true;
        }

        private double GetDistance(Vector2f a, Vector2f b)
        {
            return Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)));
        }
    }
}
