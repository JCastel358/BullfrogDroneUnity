using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;

namespace AdvancedTurrets.Examples.Shotgun
{
    [AddComponentMenu("Hidden/")]
    public class ShotgunDetail : ExampleGUI<MuzzleLoader>
    {
        protected override string GetText(MuzzleLoader t)
        {
            var result = $"{nameof(t.MuzzleCapacity)}={t.MuzzleCapacity}\n";
            result += $"{nameof(t.FireRate)}={t.FireRate}\n";
            result += $"{nameof(t.Muzzles)} {nameof(t.Muzzles.Length)}={t.Muzzles.Length}\n";
            result += $"{nameof(t.BurstInterval)}={t.BurstInterval}\n";
            return result;
        }
    }
}