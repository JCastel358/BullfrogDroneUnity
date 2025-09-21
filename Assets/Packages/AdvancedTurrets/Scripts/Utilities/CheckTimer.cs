using AdvancedTurrets.Serialization;
using UnityEngine;

namespace AdvancedTurrets.Utilities
{
    /// <summary>
    /// The CheckTimer class is a timer that can be repeatedly checked to see whether or not it has elapsed yet.
    /// Once elapsed it will need to explicitly be reset.
    /// </summary>
    [System.Serializable]
    public class CheckTimer
    {
        [Tooltip("The duration that the timer will wait for")]
        public float WaitTime = 5f;

        [Tooltip("The time that the timer began waiting (if applicable)")]
        public AdvancedNullable<float> StartTime = new();

        /// <summary
        /// Returns true whenever the wait time has elapsed
        /// </summary>
        public bool HasElapsed()
        {
            StartTime.SetIfEmpty(AdvancedTime.Time);
            return AdvancedTime.Time >= StartTime.Value + WaitTime;
        }

        /// <summary
        /// Reset the check timer to begin waiting again. 
        /// The underlying <see cref="WaitTime"> property can be changed by optionally presenting it here, otherwise it will wait for the same duration as before
        /// </summary>
        public void Reset(float? adjustedWaitTime = default)
        {
            if (adjustedWaitTime.HasValue)
            {
                WaitTime = adjustedWaitTime.Value;
            }

            StartTime.Clear();
        }
    }
}
