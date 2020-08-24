using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Shared.Controllers
{
    public class XBoxController
    {
        private Joystick joystick;

        private Dictionary<JoystickOffset, Func<int, (XBoxButton, float)>> transformFunctions;

        private HashSet<JoystickOffset> supportedButtons;

        private Dictionary<XBoxButton, WrappedJoystickUpdate> buttonPositions;

        public XBoxController(Guid joystickId)
        {
            // Set this gamepads Id
            this.Id = joystickId;

            // Initialize DirectInput
            var directInput = new DirectInput();

            this.buttonPositions = new Dictionary<XBoxButton, WrappedJoystickUpdate>();

            // Instantiate the joystick
            joystick = new Joystick(directInput, joystickId);

            this.PopulateSupportedButtons();

            this.PopulateTransformFunctions();

            this.PopulateButtonPositions();

            Debug.WriteLine("Connected to Joystick/Gamepad with GUID: {0}", joystickId);

            // Query all suported ForceFeedback effects
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
                Debug.WriteLine("Effect available {0}", effectInfo.Name);

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Acquire();
        }

        private void PopulateButtonPositions()
        {
            buttonPositions.Add(XBoxButton.X, new WrappedJoystickUpdate(XBoxButton.X, 0));
            buttonPositions.Add(XBoxButton.Y, new WrappedJoystickUpdate(XBoxButton.Y, 0));
            buttonPositions.Add(XBoxButton.RT,new WrappedJoystickUpdate(XBoxButton.RT, 0));
            buttonPositions.Add(XBoxButton.LT,new WrappedJoystickUpdate(XBoxButton.LT, 0));
        }

        private void PopulateTransformFunctions()
        {
            transformFunctions = new Dictionary<JoystickOffset, Func<int, (XBoxButton, float)>>
            {
                { JoystickOffset.X, ButtonTransforms.TransformX },
                { JoystickOffset.Y, ButtonTransforms.TransformY },
                { JoystickOffset.Z, ButtonTransforms.TransformZ },
            };
        }

        private void PopulateSupportedButtons()
        {
            supportedButtons = new HashSet<JoystickOffset>
            {
                JoystickOffset.X,
                JoystickOffset.Y,
                JoystickOffset.Z,
            };
        }

        public WrappedJoystickUpdate[] OnUpdate()
        {
            joystick.Poll();
            var bufferedData = joystick.GetBufferedData();

            // Only return the latest state of each of the buttons
            var result = bufferedData
                .Where(d => supportedButtons.Contains(d.Offset))
                .OrderBy(d => d.Timestamp)
                .ThenByDescending(d => d.Sequence)
                .GroupBy(d => d.Offset)
                .Select(v => v.First())
                .Select(v => GetWrappedValue(v.Offset, v.Value)).ToArray();

            this.UpdateButtonPositions(result);

            return buttonPositions.Values.ToArray();
        }

        private void UpdateButtonPositions(WrappedJoystickUpdate[] results)
        {
            foreach (var result in results)
            {
                buttonPositions[result.Button].Value = result.Value;
            }
        }

        private WrappedJoystickUpdate GetWrappedValue(JoystickOffset offset, int value)
        {
            var transformFunction = transformFunctions[offset];
            var (button, offsetValue) = transformFunctions[offset](value);
            return new WrappedJoystickUpdate(button, offsetValue);
        }

        public Guid Id { get; set; }

        public void Reset()
        {
            foreach (var button in buttonPositions)
            {
                button.Value.Value = 0;
            }
        }
    }
}
