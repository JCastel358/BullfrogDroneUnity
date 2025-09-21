using System;
using AdvancedTurrets.Geometrics;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Kinematics.Extensions;
using AdvancedTurrets.Libraries;
using AdvancedTurrets.Serialization;
using UnityEngine;


namespace AdvancedTurrets.Visualization.Trajectories
{
    [Serializable]
    public class InterceptFrameGizmos : BaseGizmos
    {
        [Tooltip("The number of lines used to visualize the intercept frame.")]
        public int LineCount = 1;

        [Tooltip("The radius of the spheres drawn at the intercept points.")]
        public AdvancedNullable<float> BoundaryRadius = new(0.05f);

        public void DrawGizmos(Trajectory trajectory, Trajectory relativeTo = default)
        {
            if (!Enabled || trajectory == default)
            {
                return;
            }

            Gizmos.color = MainColor;

            relativeTo ??= trajectory;
            var tFrom = relativeTo.GetRemainingTrajectoryEndTime(RemainingEndTypes.FixedFloor);
            var tTo = relativeTo.GetRemainingTrajectoryEndTime(RemainingEndTypes.FixedCeiling);
            trajectory.GetPositionsByCount(tFrom, tTo, LineCount + 1).AsLines().ForEach(AdvancedGizmos.DrawLine);

            if (BoundaryRadius.HasValue)
            {
                Gizmos.DrawSphere(trajectory.GetPosition(tFrom), BoundaryRadius);
                Gizmos.DrawSphere(trajectory.GetPosition(tTo), BoundaryRadius);
            }
        }
    }
}