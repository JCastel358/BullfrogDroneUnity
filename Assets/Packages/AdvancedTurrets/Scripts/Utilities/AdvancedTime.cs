
using AdvancedTurrets.Mathematics;

namespace AdvancedTurrets.Utilities
{
    /// <summary>
    /// A utility class that provides some different endpoints than Unity's <see cref="UnityEngine.Time"/>.
    /// There is the ability to override these values - this is used for testing and no deployed code should use these.
    /// </summary>
    public static class AdvancedTime
    {
        /// <summary>
        /// Returns Unity's <see cref="UnityEngine.Time.time"/>.
        /// </summary>
        public static float Time
        {
            get
            {
# if UNITY_EDITOR
                if (_timeOverride.HasValue)
                {
                    return _timeOverride.Value;
                }
#endif
                return UnityEngine.Time.time;
            }
        }

        /// <summary>
        /// Returns Unity's <see cref="UnityEngine.Time.fixedTime"/>.
        /// </summary>
        public static float FixedTime
        {
            get
            {
# if UNITY_EDITOR
                if (_fixedTimeOverride.HasValue)
                {
                    return _fixedTimeOverride.Value;
                }
#endif
                return UnityEngine.Time.fixedTime;
            }
        }

        /// <summary>
        /// Returns Unity's <see cref="UnityEngine.Time.fixedDeltaTime"/>.
        /// </summary>
        public static float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime;

        /// <summary>
        /// Appropriately retrieves either <see cref="UnityEngine.Time.deltaTime"/> or <see cref="UnityEngine.Time.fixedDeltaTime"/> depending on the current time.
        /// </summary>
        public static float SmartDeltaTime => InFixedTimeStep ? FixedDeltaTime : UnityEngine.Time.deltaTime;

        /// <summary>
        /// The amount of time until the next FixedUpdate. In FixedTime this is 0 as it assums it is happening presently.
        /// </summary>
        public static float TimeUntilFixedUpdate => InFixedTimeStep ? 0 : FixedTime + FixedDeltaTime - Time;

        /// <summary>
        /// Gets the time elapsed since the last fixed update.
        /// </summary>
        public static float TimeSinceFixedUpdate => InFixedTimeStep ? FixedDeltaTime : Time - FixedTime;

        /// <summary>
        /// Returns Unity's <see cref="UnityEngine.Time.inFixedTimeStep"/>.
        /// </summary>
        public static bool InFixedTimeStep
        {
# if UNITY_EDITOR
            get
            {
                if (_inFixedTimeStepOverride.HasValue)
                {
                    return _inFixedTimeStepOverride.Value;
                }
#endif

                return UnityEngine.Time.inFixedTimeStep;
            }
        }

# if UNITY_EDITOR
        # region Overrides (used internally by AT for testing/simulating purposes)

        private static float? _fixedTimeOverride;
        private static float? _timeOverride;
        public static void OverrideTime(float value)
        {
            _timeOverride = value;
            _fixedTimeOverride = AdvancedMathematics.ToIncrementalFloor(value, FixedDeltaTime);
        }

        private static bool? _inFixedTimeStepOverride;
        public static void OverrideInFixedTime(bool value)
        {
            _inFixedTimeStepOverride = value;
        }

        public static void ClearAllOverrides()
        {
            _timeOverride = default;
            _fixedTimeOverride = default;
            _inFixedTimeStepOverride = default;
        }
        #endregion
#endif
    }
}
