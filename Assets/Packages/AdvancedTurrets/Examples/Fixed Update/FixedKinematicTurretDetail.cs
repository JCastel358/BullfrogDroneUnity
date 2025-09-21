using UnityEngine;

namespace AdvancedTurrets.Examples.FixedUpdate
{
    [AddComponentMenu("Hidden/")]
    public class FixedKinematicTurretDetail : ExampleGUI<TimeAdjustableKinematicTurret>
    {
        protected override string GetText(TimeAdjustableKinematicTurret fkt)
        {
            var result = $"{nameof(fkt.UpdateTime)} = {fkt.UpdateTime}";
            return result;
        }
    }
}