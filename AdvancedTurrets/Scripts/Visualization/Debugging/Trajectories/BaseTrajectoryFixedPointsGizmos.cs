using UnityEngine;


namespace AdvancedTurrets.Visualization.Trajectories
{
    public abstract class BaseTrajectoryFixedPointsGizmos : BaseGizmos
    {
        [Tooltip("The radius of the markers used to indicate fixed time steps along the trajectory.")]
        public float Radius = 0.05f;
    }
}