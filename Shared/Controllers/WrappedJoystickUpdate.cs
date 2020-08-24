using SharpDX.DirectInput;
using System;

namespace Shared.Controllers
{
    public class WrappedJoystickUpdate
    {
        private const float NullBuffer = 0.01f;

        public WrappedJoystickUpdate(XBoxButton button, float value)
        {
            this.Button = button;

            var absValue = Math.Abs(value);

            this.Value = absValue > NullBuffer ? value : 0;
        }

        public XBoxButton Button { get; set; }

        public float Value { get; set; }
    }
}
