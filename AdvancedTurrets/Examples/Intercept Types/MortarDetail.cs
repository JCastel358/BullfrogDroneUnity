using AdvancedTurrets.Behaviours.Turrets;
using AdvancedTurrets.Kinematics;
using UnityEngine;

namespace AdvancedTurrets.Examples.InterceptTypes
{
    [AddComponentMenu("Hidden/")]
    public class MortarDetail : ExampleGUI<KinematicTurret>
    {
        protected override string GetText(KinematicTurret t)
        {
            var result = $"{nameof(t.Mortar)}={t.Mortar}\n";

            if (t.LastAimTrajectory is Trajectory trajectory)
            {
                result += $"T={trajectory.Duration:N2}s";
            }

            return result;
        }
    }
}