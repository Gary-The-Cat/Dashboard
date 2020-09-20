using Google.OrTools.LinearSolver;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.ExtensionMethods;
using Shared.Graphing;
using Shared.Interfaces;
using Shared.SharecdColors;
using System.Linq;

namespace ORToolsDemo.Screens
{
    public class ORToolsDemoScreen : Screen
    {
        private CartesianGraph graph;

        public ORToolsDemoScreen(
            IApplication application,
            IApplicationInstance applicationInstance) 
            : base(application.Configuration, applicationInstance)
        {
            graph = new CartesianGraph(application.Window.Size, new Vector2u(20, 10) * 2);
            graph.SetAxisCentred(true);

            var (p1, p2, p3) = this.GetVertices();

            graph.DrawTriangle(p1, p2, p3);

            graph.DrawLine(1, 2, 14);
            graph.DrawLine(3, -1, 0);
            graph.DrawLine(1, -1, 2);

            var result = this.SolveLinearInequalities();
            graph.DrawCircle(result);
        }

        private (Vertex, Vertex, Vertex) GetVertices()
        {
            var value1 = MaximizationFunction(2, 6);
            var value2 = MaximizationFunction(6, 4);
            var value3 = MaximizationFunction(-1, -3);

            var values = new[] { value1, value2, value3 };
            var min = values.Min();
            var max = values.Max();

            // Shitty normalization
            value1 = 1 / (max - min) * (value1 - max) + 1;
            value2 = 1 / (max - min) * (value2 - max) + 1;
            value3 = 1 / (max - min) * (value3 - max) + 1;

            var hslColor = new HslColor(180, 180, 180);

            hslColor.Saturation = value1 * 255;
            var p1 = new Vertex(new Vector2f(2, 6), hslColor);

            hslColor.Saturation = value2 * 255;
            var p2 = new Vertex(new Vector2f(6, 4), hslColor);

            hslColor.Saturation = value3 * 255;
            var p3 = new Vertex(new Vector2f(-1, -3), hslColor);

            return (p1, p2, p3);
        }

        private Vector2f SolveLinearInequalities()
        {
            // Create the linear solver with the GLOP backend.
            var solver = Solver.CreateSolver("SimpleLpProgram", "GLOP");

            // Create the variables x and y.
            var x = solver.MakeNumVar(-4, 8, "x");
            var y = solver.MakeNumVar(-4, 8, "y");

            // Create a linear constraint, x + 2y <= 14.
            var constraint1 = solver.MakeConstraint(-100, 14, "ct");
            constraint1.SetCoefficient(x, 1);
            constraint1.SetCoefficient(y, 2);

            // Create a linear constraint, 0 <= 3x - y
            var constraint2 = solver.MakeConstraint(0, 100, "ct");
            constraint2.SetCoefficient(x, 3);
            constraint2.SetCoefficient(y, -1);

            // Create a linear constraint, x - y <= 2.
            var constraint3 = solver.MakeConstraint(-100, 2, "ct");
            constraint3.SetCoefficient(x, 1);
            constraint3.SetCoefficient(y, -1);

            // Create the objective function, 3x + 4y.
            var objective = solver.Objective();
            objective.SetCoefficient(x, 3);
            objective.SetCoefficient(y, 4);
            objective.SetMaximization();

            // Solve
            solver.Solve();

            return new Vector2f((float)x.SolutionValue(), (float)y.SolutionValue());
        }

        public override void OnRender(RenderTarget target)
        {
            target.Draw(graph);
        }

        public float MaximizationFunction(float x, float y)
        {
            return 3 * x + 4 * y;
        }
    }
}
