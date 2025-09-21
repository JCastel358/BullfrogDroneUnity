using System;
using AdvancedTurrets.Utilities;
using UnityEngine;

namespace AdvancedTurrets.Kinematics.Extensions
{
    public enum RemainingEndTypes
    {
        FixedCeiling,
        Absolute,
        FixedFloor
    }

    public static class KinematicsExtensions
    {
        // todo make these more atomic
        public static void GetNetTrajectoryTime(this Trajectory trajectory, out float tFrom, out float tTo)
        {
            tTo = trajectory.Duration;
            tFrom = Mathf.Min(-trajectory.CreatedTimeUntilFixedUpdate, tTo);
        }

        public static void GetElapsedTrajectoryTime(this Trajectory trajectory, out float tFrom, out float tTo)
        {
            tTo = trajectory.GetElapsedTime();
            tFrom = Mathf.Min(-trajectory.CreatedTimeUntilFixedUpdate, tTo);
        }

        public static float GetRemainingTrajectoryEndTime(this Trajectory trajectory, RemainingEndTypes remainingEndType)
        {
            return remainingEndType switch
            {
                RemainingEndTypes.FixedCeiling => trajectory.GetDurationFixedTimeFloor() + AdvancedTime.FixedDeltaTime,
                RemainingEndTypes.Absolute => trajectory.Duration,
                RemainingEndTypes.FixedFloor => trajectory.GetDurationFixedTimeFloor(),
                _ => throw new NotImplementedException(),
            };
        }

        public static void GetRemainingTrajectoryTimes(this Trajectory trajectory, RemainingEndTypes remainingEndType, out float tFrom, out float tTo)
        {
            tFrom = trajectory.GetElapsedTime();
            tTo = Mathf.Max(GetRemainingTrajectoryEndTime(trajectory, remainingEndType), tFrom);
        }
    }
}