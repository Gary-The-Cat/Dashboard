using System;
using System.Threading;

namespace Shared.Maths
{
    public class EasingWorker
    {
        public EasingWorker(
            Func<double, double> getValue, 
            Action<double> updateValue,
            int durationMillis,
            double maxValue,
            double steps = 100)
        {
            double t = 0;
            double step = (durationMillis / steps);

            var timer = new Timer(_ =>
            {
                var value = getValue(t) * maxValue;
                t += (step/1000);
                t = t > 1 ? 1 : t;
                updateValue(value);
            }, 
            null, 
            durationMillis,
            (int)step);
        }
    }
}
