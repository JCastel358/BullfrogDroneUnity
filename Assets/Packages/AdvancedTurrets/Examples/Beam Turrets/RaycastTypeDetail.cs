using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;
using static AdvancedTurrets.Behaviours.Turrets.BeamTurret;

namespace AdvancedTurrets.Examples.BeamTurrets
{
    [AddComponentMenu("Hidden/")]
    public class RaycastTypeDetail : ExampleGUI<BeamTurret>
    {
        protected override string GetText(BeamTurret bt)
        {
            var header = $"{nameof(bt.RaycastType)}={bt.RaycastType}\n";
            switch (bt.RaycastType)
            {
                case BeamRaycastTypes.StopAtFirstHit:
                    header += "Only the first collision will be registered";
                    break;
                case BeamRaycastTypes.StopAtTarget:
                    header += "All collisions to the target point will be registered";
                    break;
                case BeamRaycastTypes.StopAtRange:
                    header += "All collisions within range will be registered";
                    break;
            }

            return header;
        }
    }
}