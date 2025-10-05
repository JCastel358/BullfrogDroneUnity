using UnityEngine;


namespace AdvancedTurrets.Visualization.Trajectories
{
    public abstract class BaseTrajectoryLinesGizmos : BaseGizmos
    {
        public enum LineTypes
        {
            FixedTime,
            FixedCount,
        }

        [Tooltip("Specifies whether the trajectory should be drawn with a fixed time interval or a fixed number of segments.")]
        public LineTypes Type = LineTypes.FixedCount;

        [Tooltip("The number of segments used when drawing the trajectory in FixedCount mode.")]
        public int FixedCount = 5;

        [Tooltip("The time interval between points when drawing the trajectory in FixedTime mode.")]
        public float FixedTime = 0.02f;
    }
}