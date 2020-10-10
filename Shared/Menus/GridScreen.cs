using SFML.Graphics;
using SFML.System;
using Shared.Events.EventArgs;
using Shared.Interfaces;
using Shared.ScreenConfig;
using System;

namespace Shared.Menus
{
    public class GridScreen
    {
        private const int MinPixelBuffer = 50;

        private Vector2f screenSize;

        private IMenuItem[,] grid;

        private Vector2f position;

        private IApplicationManager appManager;

        // Do we want to let the user set a fixed size?
        public bool AutoScale { get; }

        public GridScreen(
            IApplicationManager appManager)
        {
            this.appManager = appManager;

            this.screenSize = new Vector2f(appManager.GetWindowSize().X, appManager.GetWindowSize().Y);

            this.position = new Vector2f(0, 0);

            this.AutoScale = true;

            this.grid = new IMenuItem[1,1];
        }

        public void OnMousePress(MouseClickEventArgs args)
        {
            var relativePosition = new Vector2f(args.Args.X, args.Args.Y) - position;
            (int x, int y)? gridPosition = GetGridPosition(relativePosition);

            if (gridPosition != null)
            {
                var menuItem = grid[gridPosition.Value.x, gridPosition.Value.y];
                menuItem.OnClick?.Invoke();
            }

            args.IsHandled = true;
        }

        public void Clear()
        {
            this.grid = new IMenuItem[1, 1];
        }

        private (int, int)? GetGridPosition(Vector2f mousePosition)
        {
            // Check if the click was outside of our screen
            if (mousePosition.X < 0 || 
                mousePosition.Y < 0 ||
                mousePosition.X > screenSize.X || 
                mousePosition.Y > screenSize.Y)
            {
                return null;
            }

            RectangleShape[,] cellBounds = GetCellBounds();

            for (int x = 0; x < NumColumns; x++)
            {
                for (int y = 0; y < NumRows; y++)
                {
                    if(cellBounds[x, y] != null && cellBounds[x,y].GetGlobalBounds().Contains(mousePosition.X, mousePosition.Y))
                    {
                        return (x, y);
                    }
                }
            }

            return null;
        }

        private RectangleShape[,] GetCellBounds()
        {
            var results = new RectangleShape[NumColumns, NumRows];

            for (int x = 0; x < NumColumns; x++)
            {
                for (int y = 0; y < NumRows; y++)
                {
                    if (grid[x,y] == null)
                    {
                        results[x, y] = null;
                    }
                    else
                    {
                        // Get the position,
                        var position = GetPositionFromRowColumn(x, y);
                        var size = GetFixedMaxSize();

                        results[x, y] = new RectangleShape(size)
                        {
                            Position = position - size / 2
                        };
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// REALLY inefficient add row, but really simple conceptually.
        /// Create a new grid that has 1 more row, copy everything && replace the original.
        /// </summary>
        public void AddRow()
        {
            grid = CopyGrid(NumColumns, NumRows + 1);
        }

        public void AddColumn()
        {
            grid = CopyGrid(NumColumns + 1, NumRows);
        }

        public void AddMenuItem(int row, int column, IMenuItem menuItem)
        {
            grid[column, row] = menuItem;
        }

        // Indexing is done [x, y]
        // Column == X
        // Row == Y
        public int NumColumns => grid.GetLength(0);

        public int NumRows => grid.GetLength(1);

        public void OnRender(RenderTarget target)
        {
            target.SetView(appManager.GetDefaultView());

            var size = GetFixedMaxSize();

            for (int x = 0; x < NumColumns; x++)
            {
                for (int y = 0; y < NumRows; y++)
                {
                    var visual = grid[x, y];

                    // If the visual has not been set, skip it and draw the next.
                    if (visual == null)
                    {
                        continue;
                    }

                    visual.SetDrawableSize(size);
                    visual.SetDrawableOrigin(size / 2);
                    visual.SetDrawablePosition(GetPositionFromRowColumn(x, y));

                    target.Draw(visual.GetDrawable());
                }
            }
        }
        
        private Vector2f GetPositionFromRowColumn(int column, int row)
        {
            // Get the size of each of the grid items
            var maxItemSize = GetMaxSize();

            var b = MinPixelBuffer;
            var xSize = maxItemSize.X;
            var ySize = maxItemSize.Y;

            var xCentre = b + (b * column) + (xSize * (column) + xSize / 2);
            var yCentre = b + (b * row) + (ySize * (row) + ySize / 2);

            return new Vector2f(xCentre, yCentre);
        }

        public Vector2f GetFixedMaxSize()
        {
            var size = GetMaxSize();

            var minAxis = Math.Min(size.X, size.Y);

            return new Vector2f(minAxis, minAxis);
        }

        public Vector2f GetMaxSize()
        {
            var totalXBuffer = (NumColumns + 1) * MinPixelBuffer;
            var totalYBuffer = (NumRows + 1) * MinPixelBuffer;
            var remainingXSpace = screenSize.X - totalXBuffer;
            var remainingYSpace = screenSize.Y - totalYBuffer;

            var iconSizeX = remainingXSpace / NumColumns;
            var iconSizeY = remainingYSpace / NumRows;

            var maxAllowableSize = Math.Min(iconSizeX, iconSizeY);

            if (maxAllowableSize < MinPixelBuffer)
            {
                throw new Exception("Could not satisfy the minimum border requirements.");
            }

            return new Vector2f(iconSizeX, iconSizeY);
        }

        private IMenuItem[,] CopyGrid(int columns, int rows)
        {
            var newGrid = new IMenuItem[columns, rows];

            for (int x = 0; x < NumColumns; x++)
            {
                for (int y = 0; y < NumRows; y++)
                {
                    newGrid[x, y] = grid[x, y];
                }
            }

            return newGrid;
        }
    }
}
