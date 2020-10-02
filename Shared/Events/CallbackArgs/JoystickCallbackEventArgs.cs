using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events.CallbackArgs
{
    public class JoystickCallbackEventArgs
    {
        public JoystickCallbackEventArgs(int key)
        {
            Key = key;
        }

        public int Key { get; set; }
    }
}
