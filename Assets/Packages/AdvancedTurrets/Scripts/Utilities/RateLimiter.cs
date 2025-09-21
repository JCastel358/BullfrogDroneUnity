using UnityEngine;

namespace AdvancedTurrets.Utilities
{
    /// <summary>
    /// Rate limits and measures framerate errors to create smooth interpolation for the caller.
    /// </summary>
    [System.Serializable]
    public class RateLimiter
    {
        [Tooltip("The time the RateLimiter will measure elapsed time from")]
        public float WaitStart;

        [Tooltip("The duration the RateLimiter has to wait for it can pass")]
        public float WaitTime;

        public RateLimiter(float waitStart = 0, float waitTime = 0)
        {
            WaitStart = waitStart;
            WaitTime = waitTime;
        }

        /// <summary>
        /// Gets the time when the RateLimiter will be ready to pass.
        /// </summary>
        public float ReadyTime => WaitStart + WaitTime;

        /// <summary>
        /// Checks whether the RateLimiter can pass based on the elapsed time.
        /// </summary>
        public bool CanPass => AdvancedTime.Time >= ReadyTime;

        /// <summary>
        /// Gets the elapsed time since the RateLimiter started waiting.
        /// </summary>
        public float ElapsedTime => AdvancedTime.Time - WaitStart;

        /// <summary>
        /// Gets the remaining time until the RateLimiter can pass.
        /// </summary>
        public float RemainingTime => WaitTime - ElapsedTime;

        /// <summary>
        /// Gets the percentage of time remaining until the RateLimiter can pass.
        /// </summary>
        public float RemainingPercent => WaitTime == 0 ? 0 : Mathf.Clamp01(RemainingTime / WaitTime);

        /// <summary>
        /// Attempts to pass the RateLimiter if the required time has elapsed.
        /// </summary>
        /// <param name="deltaTime">The elapsed time since the last check.</param>
        /// <param name="waitTime">The wait time for the next pass.</param>
        /// <param name="tError">The time "missed" by this frame when passing. Use this to interpolate for continuity.</param>
        public bool Pass(float deltaTime, float waitTime, out float tError)
        {
            if (CanPass)
            {
                tError = GetContinuousError(deltaTime);
                WaitStart = AdvancedTime.Time;
                WaitTime = waitTime - tError;
                return true;
            }

            tError = default;
            return false;
        }

        /// <summary>
        /// Gets the continuous error based on the elapsed time and the ready time.
        /// </summary>
        /// <param name="deltaTime">The elapsed time since the last check.</param>
        public float GetContinuousError(float deltaTime)
        {
            var tError = AdvancedTime.Time - ReadyTime;
            if (tError <= deltaTime && tError > 0)
            {
                return tError;
            }

            return default;
        }

        /// <summary>
        /// Appends to the existing <see cref="WaitTime"/>
        /// </summary>
        public void AddWaitTime(float waitTime)
        {
            WaitTime += waitTime;
        }

        /// <summary>
        /// Clears any <see cref="WaitTime"/>. RateLimiter ready time can be specified.
        /// </summary>
        public void Reset(float readyTime = 0)
        {
            WaitStart = readyTime;
            WaitTime = 0f;
        }
    }
}
