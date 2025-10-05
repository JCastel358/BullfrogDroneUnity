using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;

namespace AdvancedTurrets.Examples.ProjectileForces
{
    [AddComponentMenu("Hidden/")]
    public class ProjectileForcesDetail : ExampleGUI<KinematicTurret>
    {
        protected override string GetText(KinematicTurret t)
        {
            var result = "";
            if (t.LastAimTrajectory != default)
            {
                result += $"A={t.LastAimTrajectory.Acceleration}\n";
                result += $"G={t.LastAimTrajectory.Gravity}\n";
            }
            return result;
        }
    }
}