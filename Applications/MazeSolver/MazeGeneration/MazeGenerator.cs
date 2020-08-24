using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeSolver.MazeGeneration
{
    public class MazeGenerator
    {
        private Random random;
        private Cell[,] cells;
        private int numRows;
        private int numCols;

        public List<Cell> mapGeneration;

        public MazeGenerator(int numRows, int numCols)
        {
            random = new Random();

            this.numRows = numRows;
            this.numCols = numCols;

            mapGeneration = new List<Cell>();
            cells = new Cell[numRows, numCols];
        }

        public void Generate()
        {
            var stack = new Stack<Cell>();

            InitializeStructures();

            // Add starting cell to top of stack, mark visited.
            stack.Push(cells[1, 1]);
            mapGeneration.Clear();
            mapGeneration.Add(cells[1, 1]);
            stack.Peek().visited = true;
            stack.Peek().blocked = false;

            while (stack.Any())
            {
                // Get current cell from top of stack.
                Cell thisCell = stack.Peek();

                // Get unvisited neighbour cells.
                List<Cell> neighbourCells = GetNeighbourCells(thisCell);

                if (neighbourCells.Any())
                {
                    // Randomly select neighbour cell.
                    Cell nextCell = neighbourCells[random.Next(neighbourCells.Count())];

                    // Get wall cell between current and neighbour cells.
                    int midRow = thisCell.row + (nextCell.row - thisCell.row) / 2;
                    int midCol = thisCell.col + (nextCell.col - thisCell.col) / 2;
                    Cell wallCell = cells[midRow, midCol];

                    // Mark neighbour and wall cells as unblocked.
                    nextCell.blocked = false;
                    wallCell.blocked = false;
                    mapGeneration.Add(wallCell);
                    mapGeneration.Add(nextCell);
                    // Add neighbour cell to top of stack, mark visited.
                    stack.Push(nextCell);

                    nextCell.visited = true;
                }
                else
                {
                    // Remove neighbourless cell from stack.
                    stack.Pop();
                }
            }
        }

        private void InitializeStructures()
        {
            mapGeneration.Clear();

            // Initialise cell array.
            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < numCols; c++)
                {
                    cells[r, c] = new Cell();
                    cells[r, c].visited = false;
                    cells[r, c].blocked = true;
                    cells[r, c].row = r;
                    cells[r, c].col = c;
                }
            }
        }

        private List<Cell> GetNeighbourCells(Cell cell)
        {
            List<Cell> neighbourCells = new List<Cell>();

            int row = cell.row;
            int col = cell.col;

            if (row > 2 && !cells[row - 2, col].visited)
            {
                neighbourCells.Add(cells[row - 2, col]);
            }
            if (col > 2 && !cells[row, col - 2].visited)
            {
                neighbourCells.Add(cells[row, col - 2]);
            }
            if (row < numRows - 3 && !cells[row + 2, col].visited)
            {
                neighbourCells.Add(cells[row + 2, col]);
            }
            if (col < numCols - 3 && !cells[row, col + 2].visited)
            {
                neighbourCells.Add(cells[row, col + 2]);
            }

            return neighbourCells;
        }

        public void DebugPrintMaze()
        {
            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < numCols; c++)
                {
                    Console.Write((cells[r, c].blocked) ? "# " : "  ");
                }
                Console.WriteLine('\n');
            }
        }

        public bool[,] GetMaze()
        {
            var maze = new bool[numRows, numCols];
            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < numCols; c++)
                {
                    maze[r, c] = cells[r, c].blocked;
                }
            }

            return maze;
        }

        public List<List<bool>> GetMazeList()
        {
            var maze = new List<List<bool>>();
            for (int r = 0; r < numRows; r++)
            {
                maze.Add(new List<bool>());
                for (int c = 0; c < numCols; c++)
                {
                    maze[r].Add(cells[r, c].blocked);
                }
            }

            return maze;
        }
    }
}