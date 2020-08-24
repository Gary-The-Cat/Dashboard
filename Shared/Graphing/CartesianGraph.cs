using SFML.Graphics;
using SFML.System;
using Shared.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Shared.Graphing
{
    public class CartesianGraph
    {
        private Vector2f screenSize;
        private Vector2u axisSize;

        private bool areAxisCentred;
        private bool areAxisVisible;

        private int xAxisDrawFrequency;
        private int yAxisDrawFrequency;

        private Color backgroundColor;
        private Color foregroundColor;

        private RectangleShape backgroundShape;
        private Vertex[] axisLines;

        private Dictionary<int, Drawable> graphElements;

        private Vector2f originOffset;

        private float axisToScreenXScale => screenSize.X / axisSize.X;
        private float axisToScreenYScale => screenSize.Y / axisSize.Y;
        private float screenToAxisXScale => axisSize.X / screenSize.X;
        private float screenToAxisYScale => axisSize.Y / screenSize.Y;

        private long minimumXAxis => areAxisCentred ? -(axisSize.X / 2) : 0;
        private long minimumYAxis => areAxisCentred ? -(axisSize.Y / 2) : 0;
        private long maximumXAxis => areAxisCentred ? (axisSize.X / 2) : axisSize.X;
        private long maximumYAxis => areAxisCentred ? (axisSize.Y / 2) : axisSize.Y;

        public CartesianGraph(Vector2u screenSize, Vector2u axisSize)
        {
            this.screenSize = new Vector2f(screenSize.X, screenSize.Y);
            this.axisSize = new Vector2u(axisSize.X, axisSize.Y);
            this.graphElements = new Dictionary<int, Drawable>();

            areAxisCentred = false;
            areAxisVisible = true;

            xAxisDrawFrequency = 1;
            yAxisDrawFrequency = 1;

            foregroundColor = new Color(57, 121, 185);
            backgroundColor = new Color(19, 93, 168);

            backgroundShape = new RectangleShape(new Vector2f(screenSize.X, screenSize.Y))
            {
                FillColor = backgroundColor
            };

            axisLines = GetAxisLines();
        }

        public void SetAxis(Vector2u axisSize)
        {
            this.axisSize.X = axisSize.X;
            this.axisSize.Y = axisSize.Y;
        }

        public void SetAxisVisibility(bool areAxisVisible)
        {
            this.areAxisVisible = areAxisVisible;
        }

        public void SetAxisDrawFrequency(int xAxisDrawFrequency, int yAxisDrawFrequency)
        {
            if(xAxisDrawFrequency == 0 || yAxisDrawFrequency == 0)
            {
                Debug.WriteLine($"Axis visibility should be controlled through {nameof(SetAxisVisibility)}.");
            }

            this.xAxisDrawFrequency = xAxisDrawFrequency;
            this.yAxisDrawFrequency = yAxisDrawFrequency;
        }

        public void SetAxisCentred(bool areAxisCentred)
        {
            this.areAxisCentred = areAxisCentred;

            if (!areAxisCentred)
            {
                originOffset = new Vector2f(0, 0);
            }
            else
            {
                originOffset = screenSize / 2;
            }

            this.axisLines = GetAxisLines();
        }

        public void SetBackgroundColour(Color backgroundColor)
        {
            this.backgroundColor = backgroundColor;

            backgroundShape.FillColor = this.backgroundColor;
        }

        public void SetForegroundColour(Color foregroundColor)
        {
            this.foregroundColor = foregroundColor;
            this.axisLines = GetAxisLines();
        }

        // Ax + By = c
        public int DrawLine(float a, float b, float c)
        {
            var intersectPoints = new List<Vector2f>();

            if (CheckIntersect(a, b, c, 0, 1, minimumXAxis, out var bottomIntersection))
            {
                intersectPoints.Add(bottomIntersection);
            }

            if (CheckIntersect(a, b, c, 0, 1, maximumXAxis, out var topIntersection))
            {
                intersectPoints.Add(topIntersection);
            }

            if (CheckIntersect(a, b, c, 1, 0, maximumYAxis, out var rightIntersection))
            {
                intersectPoints.Add(rightIntersection);
            }

            if (CheckIntersect(a, b, c, 1, 0, minimumYAxis, out var leftIntersection))
            {
                intersectPoints.Add(leftIntersection);
            }

            // We have the points where our line intersects with our screen in grid space
            VertexArray line = new VertexArray(PrimitiveType.Lines);

            foreach(var point in intersectPoints)
            {
                line.Append(new Vertex(GetScreenPosition(point), new Color(0xaa, 0xaa, 0xaa)));
            }

            var id = GetNextId();
            graphElements.Add(id, line);

            return id;
        }

        public int DrawCircle(Vector2f position, float size = 5)
        {
            var circle = new CircleShape(size)
            {
                Position = GetScreenPosition(position),
                Origin = new Vector2f(size, size),
                FillColor = Color.White,
                OutlineColor = Color.Black,
                OutlineThickness = 2
            };

            var id = GetNextId();
            graphElements.Add(id, circle);

            return id;
        }

        public bool CheckIntersect(float a1, float b1, float c1, float a2, float b2, float c2, out Vector2f intersectPoint)
        {
            intersectPoint = new Vector2f();

            float delta = a1 * b2 - a2 * b1;

            if (delta == 0)
                return false;

            float x = (b2 * c1 - b1 * c2) / delta;
            float y = (a1 * c2 - a2 * c1) / delta;

            intersectPoint.X = x;
            intersectPoint.Y = y;

            return true;
        }

        public int DrawLineSegment(Vector2f start, Vector2f end)
        {
            throw new NotImplementedException();
        }

        public int DrawTriangle(Vector2f p1, Vector2f p2, Vector2f p3)
        {
            ConvexShape triangle = new ConvexShape(3);
            triangle.SetPoint(0, GetScreenPosition(p1));
            triangle.SetPoint(1, GetScreenPosition(p2));
            triangle.SetPoint(2, GetScreenPosition(p3));

            var id = GetNextId();
            graphElements.Add(id, triangle);

            return id;
        }

        public int DrawTriangle(Vector2f p1, Vector2f p2, Vector2f p3, Func<Vector2f, Color> getPointColour)
        {
            VertexArray triangle = new VertexArray(PrimitiveType.Triangles, 3);

            triangle[0] = new Vertex(GetScreenPosition(p1), getPointColour(p1));
            triangle[1] = new Vertex(GetScreenPosition(p2), getPointColour(p2));
            triangle[2] = new Vertex(GetScreenPosition(p3), getPointColour(p3));

            var id = GetNextId();
            graphElements.Add(id, triangle);

            return id;
        }

        public int DrawTriangle(Vertex p1, Vertex p2, Vertex p3)
        {
            VertexArray triangle = new VertexArray(PrimitiveType.Triangles, 3);

            // Not entirely sure if we should be mapping the values the user gives us here
            // into screen space or we should just draw exactly what they give us.
            triangle[0] = new Vertex(GetScreenPosition(p1.Position), p1.Color);
            triangle[1] = new Vertex(GetScreenPosition(p2.Position), p2.Color);
            triangle[2] = new Vertex(GetScreenPosition(p3.Position), p3.Color);

            var id = GetNextId();
            graphElements.Add(id, triangle);

            return id;
        }

        public bool RemoveVisual(int visualId)
        {
            return graphElements.Remove(visualId);
        }

        public void Draw(RenderTarget target)
        {
            // Draw the background
            target.Draw(backgroundShape);

            // Draw the axis
            if (areAxisVisible)
            {
                target.Draw(axisLines, PrimitiveType.Lines);
            }

            foreach(var element in graphElements)
            {
                target.Draw(element.Value);
            }
        }

        public float GetScreenX(float gridX)
        {
            return gridX * axisToScreenXScale;
        }

        public float GetScreenY(float gridY)
        {
            return screenSize.Y - gridY * axisToScreenYScale;
        }

        public Vector2f GetScreenPosition(Vector2f gridPosition)
        {
            return new Vector2f(
                originOffset.X + GetScreenX(gridPosition.X),
                - originOffset.Y + GetScreenY(gridPosition.Y));
        }

        public float GetAxisX(float screenX)
        {
            return screenX * screenToAxisXScale;
        }

        public float GetAxisY(float screenY)
        {
            return screenY * screenToAxisYScale;
        }

        public Vector2f GetAxisPosition(Vector2f screenPosition)
        {
            return new Vector2f(
                GetAxisX(screenPosition.X),
                GetAxisY(screenPosition.Y));
        }

        public void ShowXAxisLabels(bool showLabels)
        {
            throw new NotImplementedException();
        }

        public void ShowYAxisLabels(bool showLabels)
        {
            throw new NotImplementedException();
        }

        public void ShowXAxisValueLabels(bool showLabels)
        {
            throw new NotImplementedException();
        }

        public void ShowYAxisValueLabels(bool showLabels)
        {
            throw new NotImplementedException();
        }

        public int GetNextId()
        {
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (!graphElements.ContainsKey(i))
                {
                    return i;
                }
            }

            return -1;
        }

        private Vertex[] GetAxisLines()
        {
            var lines = new List<Vertex>();

            for (var x = 0; x < axisSize.X; x++)
            {
                var screenX = GetScreenX(x);
                var topPoint = new Vector2f(screenX, 0);
                var bottomPoint = new Vector2f(screenX, screenSize.Y);

                lines.Add(new Vertex(topPoint, foregroundColor));
                lines.Add(new Vertex(bottomPoint, foregroundColor));
            }

            for (var y = 0; y < axisSize.Y; y++)
            {
                var screenY = GetScreenY(y);
                var leftPoint = new Vector2f(0, screenY);
                var rightPoint = new Vector2f(screenSize.X, screenY);

                lines.Add(new Vertex(leftPoint, foregroundColor));
                lines.Add(new Vertex(rightPoint, foregroundColor));
            }

            if (areAxisCentred)
            {
                var screenY = GetScreenY(axisSize.Y / 2);
                var leftPoint = new Vector2f(0, screenY);
                var rightPoint = new Vector2f(screenSize.X, screenY);

                lines.Add(new Vertex(leftPoint, Color.White));
                lines.Add(new Vertex(rightPoint, Color.White));

                var screenX = GetScreenX(axisSize.X / 2);
                var topPoint = new Vector2f(screenX, 0);
                var bottomPoint = new Vector2f(screenX, screenSize.Y);

                lines.Add(new Vertex(topPoint, Color.White));
                lines.Add(new Vertex(bottomPoint, Color.White));
            }

            return lines.ToArray();
        }
    }
}
