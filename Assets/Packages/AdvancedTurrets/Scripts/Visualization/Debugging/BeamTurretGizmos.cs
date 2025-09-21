using System;
using AdvancedTurrets.Behaviours.Actuators;
using AdvancedTurrets.Behaviours.Turrets;
using AdvancedTurrets.Geometrics;
using UnityEngine;
using static AdvancedTurrets.Behaviours.Turrets.BeamTurret;


namespace AdvancedTurrets.Visualization
{
    [Serializable]
    public class BeamTurretGizmos : BaseGizmos
    {
        public void DrawGizmos(BeamTurret beamTurret)
        {
            if (!Enabled || beamTurret == default)
            {
                return;
            }

            // Ensure a valid target trajectory is available
            if (!beamTurret.TargetingModule.GetTargetTrajectory(out var target))
            {
                return;
            }

            // Get the next end effector (muzzle) from the muzzle loader
            if (beamTurret.MuzzleLoader.PeekNextMuzzle() is not EndEffector endEffector)
            {
                if (!beamTurret.MuzzleLoader.GetNextEndEffector(out endEffector))
                {
                    return;
                }
            }

            // Determine the direction of the beam, based on the muzzle convergence
            var beamDirection = beamTurret.MuzzleConvergence ? target.Position - endEffector.Position : endEffector.Forward;
            var muzzleRay = new Ray(endEffector.transform.position, beamDirection);

            // Determine the range of the raycast based on beam type
            var range = beamTurret.GetRaycastRange(endEffector, target.Position);

            // Raycast to determine if the beam hits anything
            switch (beamTurret.RaycastType)
            {
                case BeamRaycastTypes.StopAtFirstHit:
                    if (Physics.Raycast(muzzleRay, out var raycastHit, range, beamTurret.CollisionMask))
                    {
                        range = (raycastHit.point - endEffector.Position).magnitude;
                    }
                    break;
                case BeamRaycastTypes.StopAtTarget:
                case BeamRaycastTypes.StopAtRange:
                    break;
            }

            // Set the gizmo color and draw the line representing the beam
            Gizmos.color = MainColor;
            var line = new Line(endEffector.Position, endEffector.Position + muzzleRay.direction * range);
            AdvancedGizmos.DrawLine(line);
        }
    }
}