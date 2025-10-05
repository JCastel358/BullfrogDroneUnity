using AdvancedTurrets.Behaviours.Turrets;
using AdvancedTurrets.Utilities;
using UnityEngine;

namespace AdvancedTurrets.Examples.Gunship
{
    [AddComponentMenu("Hidden/")]
    public class Gunship : MonoBehaviour
    {
        Vector3? _lastPosition;

        Vector3 _lastVelocity;

        void Update()
        {
            var position = transform.position;

            if (_lastPosition.HasValue)
            {
                var dP = position - _lastPosition.Value;
                var dT = AdvancedTime.SmartDeltaTime;

                var v = dP / dT;
                var smoothVelocity = Vector3.Lerp(_lastVelocity, v, dT);
                foreach (var kinematicTurret in GetComponentsInChildren<KinematicTurret>())
                {
                    kinematicTurret.CustomVelocity = smoothVelocity;
                }

                _lastVelocity = smoothVelocity;
            }

            _lastPosition = position;
        }
    }
}