using MazeSolver.DataStructures;
using SFML.System;
using System.Collections.Generic;

namespace MazeSolver.Interfaces
{
    public interface IMazeSolver
    {
        void Solve(Maze maze);

        List<Vector2f> GetResult();
    }
}
