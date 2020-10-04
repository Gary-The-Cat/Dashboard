using SelfDriving.DataStructures;
using SelfDriving.Interfaces;
using SFML.System;
using SFML.Window;
using Shared.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SelfDriving.Agents
{
    public class CarHuman : ICarController
    {
        private bool captureState;

        private float timesinceLastMeasurement = 0;

        private float previousTimeAlive = 0;

        private float frequencyMs = 0;

        private bool isCapturingInput = true;

        public List<float[]> StateInputMeasurements { get; set; }

        public List<float[]> StateOutputMeasurements { get; set; }

        public int SamplesCaptured { get; private set; }

        private XBoxController gameController;

        private bool UseController => gameController != null;

        public CarConfiguration Configuration { get; set; }

        public CarHuman(bool captureState, int frequencyMs)
        {
            this.captureState = captureState;
            this.frequencyMs = frequencyMs / 1000.0f;
            StateInputMeasurements = new List<float[]>();
            StateOutputMeasurements = new List<float[]>();

            var controllerManager = new ControllerManager();
            gameController = controllerManager.GetController();

            // Default the car configuration. Rarely ever changed, not even sure this needs to be exposed.
            Configuration = new CarConfiguration();
        }

        public DrivingAction GetOutput(
            float[] rayCollisions,
            Vector2f carPosition,
            float carHeading,
            Vector2f nextCheckpointPosition)
        {
            var output = new DrivingAction();

            if (UseController)
            {
                var update = gameController.OnUpdate();

                output = GetOutputFromUpdate(update);
            }
            else
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                {
                    output.Acceleration = 1;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                {
                    output.BreakingForce = 1;
                }

                output.LeftTurnForce = Keyboard.IsKeyPressed(Keyboard.Key.Left) ? 1 : 0;
                output.RightTurnForce = Keyboard.IsKeyPressed(Keyboard.Key.Right) ? 1 : 0;
            }

            if (captureState)
            {
                if (timesinceLastMeasurement > frequencyMs)
                {
                    timesinceLastMeasurement = 0;

                    var outputMeasurement = new float[4];

                    outputMeasurement[0] = output.Acceleration;
                    outputMeasurement[3] = output.BreakingForce;
                    outputMeasurement[1] = output.LeftTurnForce;
                    outputMeasurement[2] = output.RightTurnForce;

                    if (isCapturingInput)
                    {
                        StateInputMeasurements.Add(rayCollisions);
                        StateOutputMeasurements.Add(outputMeasurement);
                        SamplesCaptured++;
                    }
                }
            }

            return output;
        }

        private DrivingAction GetOutputFromUpdate(WrappedJoystickUpdate[] update)
        {
            var updateLookup = update.ToDictionary(kvp => kvp.Button, kvp => kvp.Value);

            var turnForce = updateLookup[XBoxButton.X];
            float turnLeftForce = 0;
            float turnRightForce = 0;

            if (turnForce < 0)
            {
                turnLeftForce = turnForce * -1;
            }
            else
            {
                turnRightForce = turnForce;
            }

            var action = new DrivingAction
            {
                Acceleration = updateLookup[XBoxButton.RT],
                BreakingForce = updateLookup[XBoxButton.LT],
                LeftTurnForce = turnLeftForce,
                RightTurnForce = turnRightForce,
            };

            return action;
        }

        public void Initalize(CarConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void SetFitness(float fitness)
        {
            if (captureState)
            {
                ////timesinceLastMeasurement += timeAlive - previousTimeAlive;

                ////previousTimeAlive = timeAlive;
            }
        }

        public void KillCar()
        {
            if (captureState)
            {
                // Remove the last 1 second worth of records
                StateInputMeasurements = StateInputMeasurements.SkipLast(10).ToList();
                StateOutputMeasurements = StateOutputMeasurements.SkipLast(10).ToList();
                SamplesCaptured = StateInputMeasurements.Count();
            }
        }

        public void Reset()
        {
            if (UseController)
            {
                this.gameController.OnUpdate();
                this.gameController.Reset();
            }
        }

        public bool IsCapturing()
        {
            return isCapturingInput;
        }

        public void ToggleCapture()
        {
            isCapturingInput = !isCapturingInput;
        }

        public void ResetCapture()
        {
            StateInputMeasurements.Clear();
            StateOutputMeasurements.Clear();
            SamplesCaptured = 0;
        }

        public void OnUpdate(float deltaT)
        {
            timesinceLastMeasurement += deltaT;
        }

        public double Fitness { get; set; }
    }
}
