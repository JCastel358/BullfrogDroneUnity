using System;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Kinematics.Extensions;
using AdvancedTurrets.Libraries;
using AdvancedTurrets.Utilities;
using UnityEngine;


namespace AdvancedTurrets.Visualization.Trajectories
{
    [Serializable]
    public class ElapsedTrajectoryFixedPointsGizmos : BaseTrajectoryFixedPointsGizmos
    {
        public void DrawGizmos(Trajectory trajectory, Trajectory relativeTo = default)
        {
            if (!Enabled || trajectory == default)
            {
                return;
            }

            Gizmos.color = MainColor;
            relativeTo ??= trajectory;
            relativeTo.GetElapsedTrajectoryTime(out var tFrom, out var tTo);
            trajectory.GetPositionsByInterval(tFrom, tTo, AdvancedTime.FixedDeltaTime, false).ForEach(p => Gizmos.DrawSphere(p, Radius));
        }
    }
}