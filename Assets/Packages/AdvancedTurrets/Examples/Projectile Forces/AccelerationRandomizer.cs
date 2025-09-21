using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;

namespace AdvancedTurrets.Examples.ProjectileForces
{
    [AddComponentMenu("Hidden/")]
    public class AccelerationRandomizer : MonoBehaviour
    {
        public float Magnitude = 100;
        public KinematicTurret KinematicTurret;

        public void RandomizeAcceleration()
        {
            KinematicTurret.OverrideAcceleration = Random.onUnitSphere * Magnitude;
        }
    }
}