using SelfDriving.Agents;
using SelfDriving.Interfaces;
using Shared.Interfaces;
using System.Collections.Generic;

namespace SelfDriving.Shared.RaceSimulation
{
    public class RacingSimulationLogic
    {
        internal IApplication application;

        private List<Car> cars;

        private Track track;

        public RacingSimulationLogic(IApplication application)
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

        public void SetCars(IEnumerable<ICarController> carControllers)
        {
            this.cars.Clear();

            foreach (var carController in carControllers)
            {
                // Create our car 
                var car = new Car(carController);

                // Add the car to our local collection
                this.cars.Add(car);
            }
        }

        public void ResetCars()
        {
            foreach (var car in cars)
            {
                // Set the car to its initial state
                car.SetCarStartState(track);
                car.ResetCar();
            }
        }
    }
}
