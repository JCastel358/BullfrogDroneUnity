using AdvancedTurrets.Utilities;
using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;

namespace AdvancedTurrets.Examples.Timeouts
{
    [AddComponentMenu("Hidden/")]
    public class TimeoutDetail : ExampleGUI<BaseTurret>
    {
        protected override string GetText(BaseTurret t)
        {
            var result = $"";

            if (t.TimeoutClock.StartTime.TryGet(out var startTime))
            {
                result += $"{nameof(t.TimeoutClock.WaitTime)}={t.TimeoutClock.WaitTime}";
                result += $"\nTimeout Started: {startTime}";
                var timeoutTime = startTime + t.TimeoutClock.WaitTime;
                var timeToTimeout = Mathf.Max(timeoutTime - AdvancedTime.Time, 0);
                result += $"\nTimeout In: {timeToTimeout:N2}";
            }
            else
            {
                result += "Timeout not started yet";
            }

            return result;
        }
    }
}