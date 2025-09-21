using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;

namespace AdvancedTurrets.Examples.TimeShifting
{
    [AddComponentMenu("Hidden/")]
    public class TimeShiftingDetail : ExampleGUI<MuzzleLoader>
    {
        protected override string GetText(MuzzleLoader t)
        {
            return $"{nameof(t.TimeShift)}={t.TimeShift} seconds";
        }
    }
}