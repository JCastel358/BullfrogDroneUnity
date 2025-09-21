using AdvancedTurrets.Visualization.Trajectories;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Ammunitions
{
    /// <summary>
    /// A projectile assumes all launch forces remain constant. Provides extensive gizmos for debugging and visualization.
    /// </summary>
    public class Projectile : BaseAmmunition
    {
#if UNITY_EDITOR
        [Header("Gizmos")]
        public bool ShowGizmos = true;

        public TrajectoryGizmos TrajectoryGizmos = new();

        private void OnDrawGizmos()
        {
            if (!ShowGizmos || Trajectory == default)
            {
                return;
            }

            TrajectoryGizmos.DrawGizmos(Trajectory);
        }
#endif
    }
}
