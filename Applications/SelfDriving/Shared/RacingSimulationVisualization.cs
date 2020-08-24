using SelfDriving.Agents;
using SFML.Graphics;
using Shared.CameraTools;
using Shared.Core;
using System.Collections.Generic;
using System.Linq;

namespace SelfDriving.Shared
{
    public class RacingSimulationVisualization : Screen
    {
        private List<CarVisual> carVisuals;

        private TrackVisual trackVisual;

        private Camera camera;

        private bool IsRunning => carVisuals.Any(c => c.IsRunning);

        private CarVisual TrackedCar => carVisuals.Where(c => c.IsRunning).OrderByDescending(c => c.TotalDistance).FirstOrDefault();

        public RacingSimulationVisualization(RacingSimulation simulation)
        {
            this.camera = new Camera(
                simulation.application.Configuration.SinglePlayer, 
                simulation.application.Configuration);

            this.carVisuals = new List<CarVisual>();
        }

        public override void OnUpdate(float dt)
        {
            if (!IsRunning)
            {
                return;
            }

            carVisuals.ForEach(c => c.OnUpdate(dt));

            camera.SetCentre(TrackedCar.Position, 0.1f);

            camera.Update(dt);
        }

        public override void OnRender(RenderTarget target)
        {
            target.SetView(camera.GetView());

            trackVisual.OnRender(target);

            carVisuals.ForEach(c => c.OnRender(target));
        }

        public void Reset()
        {
            carVisuals.ForEach(c => c.ResetCar());
        }

        public void InitializeCars(IEnumerable<Car> cars)
        {
            carVisuals.Clear();
            foreach (var car in cars)
            {
                carVisuals.Add(new CarVisual(car));
            }
        }

        internal void SetTrack(Track track)
        {
            this.trackVisual = new TrackVisual(track);
        }
    }
}
