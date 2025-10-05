using System;
using AdvancedTurrets.Kinematics;
using UnityEngine;


namespace AdvancedTurrets.Visualization.Trajectories
{
    [Serializable]
    public class TerminalPointArgs : BaseGizmos
    {
        [Tooltip("The radius of the point to be drawn.")]
        public float Radius = 0.05f;

        public void DrawGizmos(Trajectory trajectory, Trajectory relativeTo = default)
        {
            if (!Enabled || trajectory == default)
            {
                return;
            }

            Gizmos.color = MainColor;
            relativeTo ??= trajectory;
            var terminalPoint = trajectory.GetPosition(relativeTo.Duration);
            Gizmos.DrawSphere(terminalPoint, Radius);
        }
    }
}