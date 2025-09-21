using System;
using AdvancedTurrets.Kinematics;
using UnityEngine;


namespace AdvancedTurrets.Visualization.Trajectories
{
    [Serializable]
    public class TrajectoryExpectedPositionArgs : BaseGizmos
    {
        [Tooltip("The radius of the point to be drawn.")]
        public float Radius = 0.05f;

        public void DrawGizmos(Trajectory trajectory)
        {
            if (!Enabled || trajectory == default)
            {
                return;
            }

            Gizmos.color = MainColor;
            var pExpected = trajectory.GetPositionNow();
            Gizmos.DrawWireSphere(pExpected, Radius);
        }
    }
}