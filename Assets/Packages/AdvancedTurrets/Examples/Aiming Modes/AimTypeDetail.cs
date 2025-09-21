using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;

namespace AdvancedTurrets.Examples.AimingModes
{
    [AddComponentMenu("Hidden/")]
    public class AimTypeDetail : ExampleGUI<BaseTurret>
    {
        protected override string GetText(BaseTurret t)
        {
            var header = "";
            if (t.MuzzleLoader.AimingEndEffector)
            {
                header += $"The turret will always aim with '{t.MuzzleLoader.AimingEndEffector.name}'";
            }
            else
            {
                header += $"{nameof(t.MuzzleLoader.AimingEndEffector)} is null.\nThe turret will aim with the next muzzle in queue";
            }

            return header;
        }
    }
}