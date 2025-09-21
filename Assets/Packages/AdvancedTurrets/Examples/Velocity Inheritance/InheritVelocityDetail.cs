using AdvancedTurrets.Behaviours.Turrets;
using AdvancedTurrets.Kinematics;
using UnityEngine;

namespace AdvancedTurrets.Examples.VelocityInheritance
{
    [AddComponentMenu("Hidden/")]
    public class InheritVelocityDetail : ExampleGUI<KinematicTurret>
    {
        protected override string GetText(KinematicTurret t)
        {
            var result = $"{nameof(t.VelocityType)}={t.VelocityType}\n";

            if (t.LastAimTrajectory != default)
            {
                var launchVelocity = t.LastAimTrajectory.LaunchVelocity;
                result += $"{nameof(Trajectory.LaunchVelocity)}={launchVelocity.magnitude:N2} {launchVelocity:N0}\n";

                var inheritVelocity = t.GetProjectileInheritedVelocity(t.transform.position);
                result += $"Velocity={inheritVelocity.magnitude:N2} {inheritVelocity:N0}\n";

                var netLaunchVelocity = launchVelocity + inheritVelocity;
                result += $"Net {nameof(Trajectory.LaunchVelocity)}={netLaunchVelocity.magnitude:N2} {netLaunchVelocity:N0}\n";
            }

            return result;
        }
    }
}