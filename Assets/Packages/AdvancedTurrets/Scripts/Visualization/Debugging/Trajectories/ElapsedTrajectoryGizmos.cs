using System;
using AdvancedTurrets.Geometrics;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Kinematics.Extensions;
using AdvancedTurrets.Libraries;
using UnityEngine;


namespace AdvancedTurrets.Visualization.Trajectories
{
    [Serializable]
    public class ElapsedTrajectoryGizmos : BaseTrajectoryLinesGizmos
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
            switch (Type)
            {
                case LineTypes.FixedTime:
                    trajectory.GetPositionsByInterval(tFrom, tTo, FixedTime).AsLines().ForEach(AdvancedGizmos.DrawLine);
                    break;

                case LineTypes.FixedCount:
                    trajectory.GetPositionsByCount(tFrom, tTo, FixedCount + 1).AsLines().ForEach(AdvancedGizmos.DrawLine);
                    break;
            }

        }
    }
}