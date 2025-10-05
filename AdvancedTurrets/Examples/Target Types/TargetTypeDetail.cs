using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;

namespace AdvancedTurrets.Examples.TargetTypes
{
    [AddComponentMenu("Hidden/")]
    public class TargetTypeDetail : ExampleGUI<TargetingModule>
    {
        protected override string GetText(TargetingModule t)
        {
            var result = $"{nameof(t.Type)}={t.Type}";

            if (t.GetTargetTrajectory(out var target))
            {
                result += $"\nP={target.Position}";
                if (target.Velocity != default)
                {
                    result += $"\nV={target.Velocity}";
                }
                if (target.Acceleration != default)
                {
                    result += $"\nA={target.Acceleration}";
                }
            }

            return result;
        }
    }
}