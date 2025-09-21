using System;
using System.Collections.Generic;
using AdvancedTurrets.Behaviours.Turrets;
using AdvancedTurrets.Geometrics;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Kinematics.Extensions;
using AdvancedTurrets.Libraries;
using UnityEngine;


namespace AdvancedTurrets.Visualization
{
    [Serializable]
    public class KinematicTurretGizmos : BaseGizmos
    {
        [Tooltip("The number of arcs to be drawn to represent spread variation.")]
        public int ArcCount = 10;

        [Tooltip("The number of trajectory lines within each arc.")]
        public int ArcLineCount = 10;

        [Tooltip("Defines how far the spread visualization extends (absolute, fixed floor, or fixed ceiling).")]
        public RemainingEndTypes EndType = RemainingEndTypes.Absolute;

        public object TrajectoryExtensions { get; private set; }

        public void DrawGizmos(KinematicTurret kinematicTurret)
        {
            if (!Enabled || kinematicTurret == default)
            {
                return;
            }

            // If no trajectory data is available, return.
            if (kinematicTurret.LastAimTrajectory is not Trajectory trajectory)
            {
                return;
            }

            // If spread is zero, simply draw the main trajectory.
            if (kinematicTurret.Spread == 0)
            {
                Gizmos.color = MainColor;
                trajectory.GetRemainingTrajectoryTimes(EndType, out var tFrom, out var tTo);
                trajectory.GetPositionsByCount(tFrom, tTo, ArcLineCount + 1).AsLines().ForEach(AdvancedGizmos.DrawLine);
                return;
            }

            // Set the Gizmos color for drawing.
            Gizmos.color = MainColor;

            // Determine the total trajectory duration rounded to fixed update intervals.
            var t = Mathf.Ceil(trajectory.Duration / .02f) * .02f;

            // Define the initial launch direction.
            var d0 = Quaternion.LookRotation(
                trajectory.LaunchVelocity != default ? trajectory.LaunchVelocity : Vector3.forward,
                Vector3.up
            );

            // Get the starting position.
            var p0 = trajectory.Position;

            // List to store the end points of each arc.
            var endPoints = new List<Vector3>();

            // Loop to create spread arcs.
            for (var i = 0f; i < ArcCount; i++)
            {
                // Calculate the rotation for this arc.
                var iDeg = i / ArcCount * 360;
                var iV0 = d0.RotateZ(iDeg).RotateY(kinematicTurret.Spread) * Vector3.forward * trajectory.LaunchVelocity.magnitude;

                // Generate the trajectory for this arc.
                var iTrajectory = new Trajectory(p0, iV0, trajectory.HasGravity, trajectory.Acceleration, t, true);

                // Get the trajectory time range for rendering.
                iTrajectory.GetElapsedTrajectoryTime(out var tFrom, out _);
                iTrajectory.GetRemainingTrajectoryTimes(EndType, out var _, out var tTo);

                // Store the endpoint and draw each arc.
                var endPoint = Vector3.zero;
                foreach (var iLine in iTrajectory.GetPositionsByCount(tFrom, tTo, ArcLineCount + 1).AsLines())
                {
                    AdvancedGizmos.DrawLine(iLine);
                    endPoint = iLine.To;
                }
                endPoints.Add(endPoint);
            }

            // Draw the outer boundary of the spread.
            foreach (var line in endPoints.AsLines(true))
            {
                AdvancedGizmos.DrawLine(line);
            }
        }
    }
}