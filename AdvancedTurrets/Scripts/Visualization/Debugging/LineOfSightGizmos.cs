using AdvancedTurrets.Behaviours.Collisions;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Libraries;
using AdvancedTurrets.Mathematics;
using AdvancedTurrets.Serialization;
using UnityEngine;

namespace AdvancedTurrets.Visualization
{
    public class LineOfSightGizmos : BaseGizmos
    {
        [Tooltip("When spherecasting line of sight, draw circles indicating the diameter of the sphere cast along the trajectory.")]
        public bool DrawSpherecastingCircles = true;

        [Tooltip("Distance between sampled trajectory points for visualization.")]
        public AdvancedNullable<float> CustomSpacing = new(1f, false);

        public void DrawGizmos(LineOfSightChecker lineOfSightChecker)
        {
            if (!Enabled || lineOfSightChecker == default)
            {
                return;
            }

            if (lineOfSightChecker.LastTrajectory is not Trajectory lastTrajectory)
            {
                return;
            }

            var dist = 0f;

            var lines = lineOfSightChecker.GetRaycastingLines(lineOfSightChecker.LastTrajectory);
            foreach (var line in lineOfSightChecker.GetRaycastingLines(lastTrajectory))
            {
                var raycastHits = lineOfSightChecker.GetRaycastHits(line);

                if (raycastHits.FindFirst(rch => lineOfSightChecker.IsRaycastHitInvalid(lastTrajectory, rch), out var invalidRaycastHit))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(invalidRaycastHit.point, lineOfSightChecker.SphereCastRadius);
                }
                else
                {
                    Gizmos.color = Color.green;
                }

                AdvancedGizmos.DrawLine(line);

                if (DrawSpherecastingCircles)
                {
                    if (CustomSpacing.HasValue)
                    {
                        while (line.EvaluateAt(dist, out var pnt))
                        {
                            if (CustomSpacing.Value.LessThanOrEqual(0f))
                            {
                                Debug.LogWarning($"Custom spacing must be a positive value.");
                                break;
                            }

                            if (lineOfSightChecker.SphereCastRadius.HasValue)
                            {
                                AdvancedGizmos.DrawCircle(pnt, line.Direction, Vector3.up, lineOfSightChecker.SphereCastRadius, 25);
                            }
                            else
                            {
                                Gizmos.DrawSphere(pnt, .1f);
                            }

                            dist += CustomSpacing;
                        }

                        // Continue evaluation after the line offset
                        dist = dist - line.Magnitude;
                    }
                    else
                    {
                        AdvancedGizmos.DrawCircle(line.To, line.Direction, Vector3.up, lineOfSightChecker.SphereCastRadius, 25);
                    }
                }
            }
        }
    }
}