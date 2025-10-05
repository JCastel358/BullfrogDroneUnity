using System.Linq;
using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Geometrics;
using AdvancedTurrets.Kinematics.Extensions;

namespace AdvancedTurrets.Examples.Spread
{
    [AddComponentMenu("Hidden/")]
    public class SpreadDetail : ExampleGUI<KinematicTurret>
    {
        protected override string GetText(KinematicTurret t)
        {
            var result = $"{nameof(t.Spread)}=±{t.Spread}°\n";

            if (t.LastAimTrajectory is Trajectory trajectory)
            {
                result += $"Duration={trajectory.Duration:N2}s\n";

                trajectory.GetNetTrajectoryTime(out var tFrom, out var tTo);
                var d = trajectory.GetPositionsByCount(tFrom, tTo, 51).AsLines().Sum(l => l.Magnitude);
                result += $"Distance={d:N2}m\n";

                var r = d * Mathf.Tan(t.Spread * Mathf.Deg2Rad);
                result += $"Max Radial Deviation={r:N2}m\n";

                var a = Mathf.PI * r * r;
                result += $"Max Area={a:N1}m^2";
            }

            return result;
        }
    }
}