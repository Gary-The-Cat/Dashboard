using System;
using System.Threading;

namespace Shared.Maths
{
    public class EasingWorker
    {
        public bool IsAlive => timeAlive < duration; 

        private Func<double, double> getValue;

        private Action<double> setValue;

        private float duration;

        private float timeAlive;

        private float minValue;

        private float difference;

        public EasingWorker(
            Func<double, double> getValue, 
            Action<double> setValue,
            float durationSeconds,
            float minValue = 0,
            float maxValue = 1)
        {
            this.getValue = getValue;
            this.setValue = setValue;
            this.duration = durationSeconds;
            this.minValue = minValue;
            this.difference = maxValue - minValue;
            timeAlive = 0;
        }

        public void OnUpdate(float deltaT)
        {
            if (!IsAlive)
            {
                return;
            }

            timeAlive += deltaT;

            var proportion = timeAlive / duration;

            var scale = getValue(proportion);

            var newValue = minValue + (scale * difference);

            setValue(newValue);
        }
    }
}
