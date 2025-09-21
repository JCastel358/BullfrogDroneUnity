using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedTurrets.Libraries;
using AdvancedTurrets.Utilities;
using AdvancedTurrets.Mathematics;
using UnityEngine;

namespace AdvancedTurrets.Kinematics
{
    /// <summary>
    /// Represents the trajectory of a moving object in 3D space.
    /// Provides an easy API to create trajectory interceptions.
    /// This is the core functionality for simulating physics-based projectiles and intercepting moving targets in AdvancedTurrets.
    /// </summary>
    [Serializable]
    public class Trajectory
    {
        [Tooltip("The position at the TimeCreated")]
        public Vector3 Position;

        [Tooltip("The velocity at the TimeCreated")]
        public Vector3 Velocity;

        [Tooltip("Whether or not the object will have gravitational forces applied by the physics engine over its lifetime")]
        public bool HasGravity;

        [Tooltip("The constant physics gravity that will be applied to this over its lifetime")]
        public Vector3 Gravity;

        [Tooltip("The constant acceleration that will be applied to this over its lifetime from FixedUpdate")]
        public Vector3 Acceleration;

        [Tooltip("The time from the projectile being launched to the time that the projectile will intercept the target")]
        public float Duration;

        [Tooltip("Whether or not the object this represents was instantiated or enabled within the same frame that this was also created in")]
        public bool InstantiatedOrEnabled;

        [Tooltip("A reference to the corresponding GameObject's Transform that this represents")]
        public Transform Transform;

        [SerializeReference]  // SerializeField doesn't capture the potential recursion here, so SerializeReference is used.
        [Tooltip("The Trajectory of the target that is being intercepted")]
        public Trajectory TargetTrajectory;

        [Tooltip("This can differ from the Trajectory's (Net) Velocity.")]
        public Vector3 LaunchVelocity;

        [Tooltip("The time this object was created")]
        public float TimeCreated;

        [Tooltip("True: this was created within the FixedUpdate cycle. False: this was created in anything other than the FixedUpdate cycle")]
        public bool CreatedInFixedTime;

        [Tooltip("From when this was created, this is the time remaining until the next FixedUpdate is expected to take place.")]
        public float CreatedTimeUntilFixedUpdate;

        [Tooltip("From when this was created, this is the time elapsed since the previous FixedUpdate took place.")]
        public float CreatedTimeSinceFixedUpdate;

        /// <param name="position">The position of the object.</param>
        /// <param name="velocity">The velocity of the object.</param>
        /// <param name="hasGravity">Whether or not Physics gravity will affecting the object.</param>
        /// <param name="acceleration">The constant acceleration applied to the object.</param>
        /// <param name="duration">The duration that this trajectory will exist for.</param>
        /// <param name="instantiatedOrEnabled">Whether or not the object was instantiated or enabled in the same frame it is was measured.</param>
        /// <param name="transform">A reference to a Transform that this Trajectory is representing (if applicable).</param>
        /// <param name="targetTrajectory">The Trajectory that this trajectory was created to intercept (if applicable).</param>
        /// <param name="launchVelocity">The launched velocity (of a fixed magnitude/speed) of this trajectory (if applicable).</param>
        /// <param name="timeShift">The amount of time that this trajectory has been shifted by.</param>
        public Trajectory(Vector3 position, Vector3 velocity, bool hasGravity, Vector3 acceleration, float duration = default, bool instantiatedOrEnabled = false, Transform transform = default, Trajectory targetTrajectory = default, Vector3 launchVelocity = default, float timeShift = default)
        {
            Position = position;
            Velocity = velocity;
            HasGravity = hasGravity;
            Gravity = HasGravity ? Physics.gravity : default;
            Acceleration = acceleration;
            Duration = duration;
            InstantiatedOrEnabled = instantiatedOrEnabled;
            Transform = transform;
            TargetTrajectory = targetTrajectory;
            LaunchVelocity = launchVelocity;
            TimeShift = timeShift;
            TimeCreated = AdvancedTime.Time;
            CreatedInFixedTime = AdvancedTime.InFixedTimeStep;
            CreatedTimeUntilFixedUpdate = AdvancedTime.TimeUntilFixedUpdate;
            CreatedTimeSinceFixedUpdate = AdvancedTime.TimeSinceFixedUpdate;
        }

        /// <summary>
        /// Creates a trajectory from a component.
        /// This will use the components position and transform as a baseline.
        /// </summary>
        /// <param name="component">The Component whose position and transform will be used.</param>
        /// <param name="velocity">The velocity of the object.</param>
        /// <param name="hasGravity">Whether or not Physics gravity will affecting the object.</param>
        /// <param name="acceleration">The constant acceleration applied to the object.</param>
        /// <param name="duration">The duration that this trajectory will exist for.</param>
        /// <param name="instantiatedOrEnabled">Whether or not the object was instantiated or enabled in the same frame it is was measured.</param>
        /// <param name="targetTrajectory">The Trajectory that this trajectory was created to intercept (if applicable).</param>
        /// <param name="launchVelocity">The launched velocity (of a fixed magnitude/speed) of this trajectory (if applicable).</param>
        /// <param name="timeShift">The amount of time that this trajectory has been shifted by.</param>
        public Trajectory(Component component, Vector3 velocity, bool hasGravity, Vector3 acceleration, float duration = default, bool instantiatedOrEnabled = false, Trajectory targetTrajectory = default, Vector3 launchVelocity = default, float timeShift = default) :
            this(component.transform.position, velocity, hasGravity, acceleration, duration, instantiatedOrEnabled, component.transform, targetTrajectory, launchVelocity, timeShift)
        {

        }

        /// <summary>
        /// Creates a trajectory from a rigidbody.
        /// This will use the rigidbody's position, velocity, gravity, and transform as a baseline.
        /// </summary>
        /// <param name="rigidbody">The Rigidbody whose position, velocity, gravity, and transform will be used.</param>
        /// <param name="acceleration">The constant acceleration applied to the object.</param>
        /// <param name="duration">The duration that this trajectory will exist for.</param>
        /// <param name="instantiatedOrEnabled">Whether or not the object was instantiated or enabled in the same frame it is was measured.</param>
        /// <param name="targetTrajectory">The Trajectory that this trajectory was created to intercept (if applicable).</param>
        /// <param name="launchVelocity">The launched velocity (of a fixed magnitude/speed) of this trajectory (if applicable).</param>
        /// <param name="timeShift">The amount of time that this trajectory has been shifted by.</param>
        public Trajectory(Rigidbody rigidbody, Vector3 acceleration, float duration = default, bool instantiatedOrEnabled = false, Trajectory targetTrajectory = default, Vector3 launchVelocity = default, float timeShift = default) :
            this(rigidbody.transform, rigidbody.GetVelocity(), rigidbody.useGravity, acceleration, duration, instantiatedOrEnabled, targetTrajectory, launchVelocity, timeShift)
        {

        }

        /// <summary>
        /// This is the duration FixedTime immediately before the fixed frame that will contain the termination of this trajectory.
        /// </summary>
        public float GetDurationFixedTimeFloor()
        {
            return AdvancedMathematics.ToIncrementalFloor(Duration, AdvancedTime.FixedDeltaTime);
        }

        /// <summary>
        /// Attempts to intercept a given target trajectory.
        /// </summary>
        /// <param name="trajectory">The resulting trajectory of the intercept if successful.</param>
        /// <param name="speed">The speed of the projectile being fired.</param>
        /// <param name="fromPosition">The starting position of the projectile.</param>
        /// <param name="withVelocity">The inherited velocity of the projectile.</param>
        /// <param name="withGravity">Whether or not Physics gravity will affect the projectile.</param>
        /// <param name="withAcceleration">The constant acceleration applied to the projectile.</param>
        /// <param name="spread">The spread (randomization) applied to the projectiles trajectory.</param>
        /// <param name="tShift">Time shift to interpolate trajectory interception by (used for frame rate continuity).</param>
        /// <param name="timeSelector">A function to select the preferred intercept trajectory if there are more than 1 option.</param>
        public bool Intercept(out Trajectory trajectory, float speed, Vector3 fromPosition, Vector3 withVelocity, bool withGravity, Vector3 withAcceleration, float spread = 0, float tShift = 0, bool preferShortestTime = true)
        {
            var p_1 = GetPosition(-tShift);
            var v_1 = AdvancedKinematics.GetDeltaTimeVelocity(-tShift, Velocity, Acceleration, Gravity);

            var a_1 = Acceleration;
            var g_1 = Gravity;

            var p_2 = fromPosition;
            var v_2 = withVelocity;

            var a_3 = withAcceleration;
            var g_3 = withGravity ? Physics.gravity : default;

            Vector3 a;
            Vector3 b;
            Vector3 c;

            if (!CreatedInFixedTime)
            {
                a = 0.5f * (a_1 + g_1) - 0.5f * (a_3 + g_3);
                b = .01f * (a_1 + g_1) + v_1 - .01f * (a_3 + g_3) - v_2;
                c = p_1 - p_2;
            }
            else if (!InstantiatedOrEnabled)
            {
                a = 0.5f * a_1 + 0.5f * g_1 - 0.5f * a_3 - 0.5f * g_3;
                b = 0.01f * a_1 + 0.01f * g_1 + v_1 + .01f * a_3 - .01f * g_3 - v_2;
                c = p_1 - p_2;
            }
            else
            {
                a = 0.5f * a_1 + 0.5f * g_1 - 0.5f * a_3 - 0.5f * g_3;
                b = -0.01f * a_1 + 0.01f * g_1 + v_1 + 0.01f * a_3 - 0.01f * g_3 - v_2;
                c = p_1 - p_2;
            }

            var t4 = Vector3.Dot(a, a);
            var t3 = 2 * Vector3.Dot(a, b);
            var t2 = 2 * Vector3.Dot(a, c) + Vector3.Dot(b, b) - (speed * speed);
            var t1 = 2 * Vector3.Dot(b, c);
            var t0 = Vector3.Dot(c, c);

            var realResults = AdvancedMathematics.Quartic(t4, t3, t2, t1, t0)
                .Where(c => c.Imaginary == 0 && c.Real >= 0)
                .Select(c => (float)c.Real);

            if (realResults.Any())
            {
                var duration = preferShortestTime ? realResults.Min() : realResults.Max();
                var launchVelocity = ((a * (duration * duration) + b * duration + c) / duration).RandomizeDirection(spread);
                var velocity = v_2 + launchVelocity;
                trajectory = new Trajectory(p_2, velocity, withGravity, withAcceleration, duration, true, default, this, launchVelocity);
                trajectory.ShiftTime(tShift);
                return true;
            }

            trajectory = default;
            return false;
        }

        # region Time Shifting

        [Tooltip("The amount of time that this has been interpolated by")]
        public float TimeShift;

        /// <summary>
        /// Shifts the trajectory by a given amount of time.
        /// </summary>
        public void ShiftTime(float time)
        {
            if (time == default)
            {
                return;
            }

            Position = AdvancedKinematics.GetPosition(time, Position, Velocity, Acceleration, Gravity, CreatedInFixedTime, InstantiatedOrEnabled);
            Velocity = AdvancedKinematics.GetDeltaTimeVelocity(time, Velocity, Acceleration, Gravity);
            TimeShift += time;
            Duration -= time;
        }

        #endregion

        # region Position

        /// <summary>
        /// Gets the position of the trajectory at a specific time.
        /// </summary>
        public Vector3 GetPosition(float elapsedTime)
        {
            return AdvancedKinematics.GetPosition(elapsedTime, Position, Velocity, Acceleration, Gravity, CreatedInFixedTime, InstantiatedOrEnabled);
        }

        /// <summary>
        /// The point in space where this trajectory will terminate based upon its duration.
        /// </summary>
        public Vector3 GetTerminalPoint()
        {
            return GetPosition(Duration);
        }

        /// <summary>
        /// Enumerates positions of the trajectory between two times in even intervels. Spacing is determined by count.
        /// </summary>
        public IEnumerable<Vector3> GetPositionsByCount(float fromTime, float toTime, int count, bool canSimplify = true)
        {
            if (canSimplify && Acceleration == default && !HasGravity)
            {
                yield return GetPosition(fromTime);
                yield return GetPosition(toTime);
            }
            else
            {
                var total = count - 1;
                var tSpan = toTime - fromTime;
                for (int i = 0; i <= total; i++)
                {
                    var t = fromTime + tSpan * i / total;
                    yield return GetPosition(t);
                }
            }
        }

        /// <summary>
        /// Enumerates positions of the trajectory between two times, with a specified time interval between each position.
        /// </summary>
        public IEnumerable<Vector3> GetPositionsByInterval(float fromTime, float toTime, float timeInterval, bool canSimplify = true)
        {
            if (Acceleration == default && !HasGravity && canSimplify)
            {
                yield return GetPosition(fromTime);
                yield return GetPosition(toTime);
            }
            else
            {
                foreach (var t in fromTime.EnumerateInterval(toTime, Mathf.Max(AdvancedTime.FixedDeltaTime, timeInterval)))
                {
                    yield return GetPosition(t);
                }
            }
        }

        #endregion

        # region Velocity

        /// <summary>
        /// Gets the velocity of the trajectory at a specific time.
        /// </summary>
        public Vector3 GetVelocity(float elapsedTime)
        {
            return AdvancedKinematics.GetVelocity(elapsedTime, Velocity, Acceleration, Gravity, CreatedInFixedTime, InstantiatedOrEnabled);
        }

        /// <summary>
        /// Enumerates velocities of the trajectory between two times in even intervels. Spacing is determined by count.
        /// </summary>
        public IEnumerable<Vector3> GetVelocitiesByCount(float fromTime, float toTime, int count, bool canSimplify = true)
        {
            if (canSimplify && Acceleration == default && !HasGravity)
            {
                yield return GetVelocity(fromTime);
            }
            else
            {
                var total = count - 1;
                var tSpan = toTime - fromTime;
                for (int i = 0; i <= total; i++)
                {
                    var t = fromTime + tSpan * i / total;
                    yield return GetVelocity(t);
                }
            }
        }

        /// <summary>
        /// Enumerates velocities of the trajectory between two times, with a specified time interval between each position.
        /// </summary>
        public IEnumerable<Vector3> GetVelocitiesByInterval(float fromTime, float toTime, float timeInterval, bool canSimplify = true)
        {
            if (Acceleration == default && !HasGravity && canSimplify)
            {
                yield return GetVelocity(fromTime);
                yield return GetVelocity(toTime);
            }
            else
            {
                foreach (var t in fromTime.EnumerateInterval(toTime, Mathf.Max(AdvancedTime.FixedDeltaTime, timeInterval)))
                {
                    yield return GetVelocity(t);
                }
            }
        }

        #endregion

        # region Present Time

        /// <summary>
        /// This is the error capturing the fixed time error from when this object was instantiated and the current frame interpreting it.
        /// This is the foundation on which all gizmos and real-time checks can operate across time frames.
        /// </summary>
        public float GetFixedTimeOffset()
        {
            return CreatedTimeSinceFixedUpdate - AdvancedTime.TimeSinceFixedUpdate;
        }

        /// <summary>
        /// The amount of time remaining on this Trajectory respecting any fixed time offsets from the invoking frame.
        /// </summary>
        public float GetRemainingTime()
        {
            return TimeCreated + Duration - AdvancedTime.Time - GetFixedTimeOffset();
        }

        /// <summary>
        /// Indicates whether or not this Trajectory has elapsed at the current time respecting any fixed time offsets from the invoking frame.
        /// </summary>
        public bool HasElapsed()
        {
            return GetRemainingTime().LessThanOrEqual(0f);
        }

        /// <summary>
        /// Gets the elapsed time at the current time respecting any fixed time offsets from the invoking frame.
        /// </summary>
        public float GetElapsedTime()
        {
            return AdvancedTime.Time - TimeCreated + GetFixedTimeOffset();
        }

        /// <summary>
        /// Gets the expected position of this at the current time respecting any fixed time offsets from the invoking frame.
        /// </summary>
        public Vector3 GetPositionNow()
        {
            return GetPosition(GetElapsedTime());
        }

        /// <summary>
        /// Gets the expected velocity of this at the current time respecting any fixed time offsets from the invoking frame.
        /// </summary>
        public Vector3 GetVelocityNow()
        {
            return GetVelocity(GetElapsedTime());
        }

        #endregion
    }
}
