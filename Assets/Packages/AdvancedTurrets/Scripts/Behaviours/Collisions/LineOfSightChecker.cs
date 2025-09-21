using System.Collections.Generic;
using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Geometrics;
using AdvancedTurrets.Visualization;
using AdvancedTurrets.Serialization;
using AdvancedTurrets.Kinematics.Extensions;

namespace AdvancedTurrets.Behaviours.Collisions
{
    /// <summary>
    /// Determines if a trajectory has a clear line of sight to its target.
    /// Utilizes raycasts or spherecasts to detect obstacles and informs whether a turret should fire.
    /// If this script is not enabled, it won't check line of sight
    /// </summary>
    public class LineOfSightChecker : MonoBehaviour
    {
        [Tooltip("The layer mask used for raycasting.")]
        public LayerMask RaycastLayerMask = -1;

        [Tooltip("The number of raycast lines when sampling the trajectory for collisions.")]
        public int RaycastLineCount = 10;

        [Tooltip("If this has a value, raycasting will be a spherecast of the specified radius. Otherwise it will be simple line raycasting.")]
        public AdvancedNullable<float> SphereCastRadius = new(0.1f);

        [Tooltip("Should this consider collisions with ones own hierarchal objects as invalid?")]
        public bool IgnoreCollisionsWithSelfHierarchy = true;

        [Tooltip("Should this consider collisions with hierarchal objects of the target as valid?")]
        public bool AllowCollisionsWithTargetHierarchy = true;

        [Tooltip("The last Trajectory that this checked line of sight for.")]
        public Trajectory LastTrajectory;

        public void CheckLineOfSight(BeforeKinematicTurretFiredArgs beforeFireArgs)
        {
            if (!enabled)
            {
                return;
            }

            LastTrajectory = beforeFireArgs.Trajectory;
            foreach (var line in GetRaycastingLines(LastTrajectory))
            {
                foreach (var raycastHit in GetRaycastHits(line))
                {
                    if (IsRaycastHitInvalid(LastTrajectory, raycastHit))
                    {
                        beforeFireArgs.CancellationToken.RequestCancellation();
                        return;
                    }
                }
            }
        }

        public IEnumerable<Line> GetRaycastingLines(Trajectory trajectory)
        {
            trajectory.GetNetTrajectoryTime(out var tFrom, out var tTo);
            return trajectory.GetPositionsByCount(tFrom, tTo, RaycastLineCount + 1).AsLines();
        }

        public IEnumerable<RaycastHit> GetRaycastHits(Line line)
        {
            if (SphereCastRadius.HasValue)
            {
                return line.SphereCastAll(SphereCastRadius, RaycastLayerMask);
            }

            return line.RaycastAll(RaycastLayerMask);
        }

        public bool IsRaycastHitInvalid(Trajectory trajectory, RaycastHit raycastHit)
        {
            // Check/ignore self collisions
            var raycastHitSelfHierarchy = raycastHit.transform.root == transform.root;
            if (IgnoreCollisionsWithSelfHierarchy && raycastHitSelfHierarchy)
            {
                return false;
            }

            // Check/ignore target hierarchy collisions
            if (trajectory.TargetTrajectory?.Transform is Transform targetTransform)
            {
                var raycastHitTargetHierarchy = raycastHit.transform.root == targetTransform.root;
                if (AllowCollisionsWithTargetHierarchy && raycastHitTargetHierarchy)
                {
                    return false;
                }

                // Actually hitting target is acceptable
                if (targetTransform == raycastHit.transform)
                {
                    return true;
                }
            }

            return true;
        }

#if UNITY_EDITOR
        [Header("Gizmos")]
        public bool ShowGizmos = true;

        public LineOfSightGizmos LineOfSightGizmos = new();

        void OnDrawGizmos()
        {
            if (!ShowGizmos)
            {
                return;
            }

            LineOfSightGizmos.DrawGizmos(this);
        }
#endif
    }
}
