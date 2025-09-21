using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;

namespace AdvancedTurrets.Examples.FireInterval
{
    [AddComponentMenu("Hidden/")]
    public class FireIntervalDetail : ExampleGUI<MuzzleLoader>
    {
        protected override string GetText(MuzzleLoader t)
        {
            var result = $"{nameof(t.BurstInterval)} = {t.BurstInterval}";
            result += $"\n{nameof(t.BurstTime)} = {t.BurstTime}s (x{t.Muzzles.Length})";
            result += $"\n{nameof(t.ReloadTime)} = {t.ReloadTime}s";
            result += $"\n{nameof(t.CycleTime)} = {t.CycleTime}s";
            return result;
        }
    }
}