using SelfDriving.Agents;
using SelfDriving.Interfaces;
using Shared.Interfaces;
using System.Collections.Generic;

namespace SelfDriving.Shared
{
    public class RacingSimulation
    {
        internal IApplication application;

        private RacingSimulationVisualization visualization;

        private List<Car> cars;

        private Track track;

        public RacingSimulation(IApplication application)
        {
            this.application = application;

            cars = new List<Car>();
        }

        public void Reset()
        {
            cars.ForEach(c =>
            {
                c.ResetCar();
                c.SetCarStartState(track);
            });
        }

        public void OnUpdate(float deltaT)
        {
            cars.ForEach(c => c.OnUpdate(deltaT));
        }

        public List<Car> GetCars()
        {
            return cars;
        }

        internal void SetTrack(Track track)
        {
            this.track = track;
        }

        public void InitializeCars(IEnumerable<ICarAI> carAIs)
        {
            this.cars.Clear();

            foreach(var carAI in carAIs)
            {
                // Create our car 
                var car = new Car(carAI);

                // Set the car to its initial state
                car.SetCarStartState(track);
                car.ResetCar();

                // Add the car to our local collection
                this.cars.Add(car);
            }
        }
    }
}
