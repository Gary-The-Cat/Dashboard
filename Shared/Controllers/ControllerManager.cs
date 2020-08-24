using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Controllers
{
    public class ControllerManager
    {
        private List<Guid> joystickIds;

        public ControllerManager()
        {
            var directInput = new DirectInput();
            joystickIds = new List<Guid>();

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad,
                        DeviceEnumerationFlags.AllDevices))
                joystickIds.Add(deviceInstance.InstanceGuid);
        }

        public XBoxController GetController()
        {
            XBoxController controller = null;

            if (joystickIds.Any())
            {
                controller = new XBoxController(joystickIds.First());
            }

            return controller;
        }
    }
}
