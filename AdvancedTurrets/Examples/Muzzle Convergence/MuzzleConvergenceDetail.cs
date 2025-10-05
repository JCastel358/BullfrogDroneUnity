using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;

namespace AdvancedTurrets.Examples.MuzzleConvergence
{
    [AddComponentMenu("Hidden/")]
    public class MuzzleConvergenceDetail : ExampleGUI<BaseTurret>
    {
        protected override string GetText(BaseTurret t)
        {
            var result = $"{nameof(t.MuzzleConvergence)}={t.MuzzleConvergence}";

            if (t.MuzzleConvergence)
            {
                result += $"All shots fired will converge towards the target\nregardless of the respective muzzle's orientation";
            }
            else
            {
                result += $"Muzzles will shoot relative to their forward axis.\nThis means projectiles may miss small targets!";
            }

            return result;
        }
    }
}