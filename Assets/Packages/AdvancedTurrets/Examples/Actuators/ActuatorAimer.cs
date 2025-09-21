using AdvancedTurrets.Behaviours.Actuators;
using AdvancedTurrets.Utilities;
using UnityEngine;

namespace AdvancedTurrets.Examples.Actuators
{
    [AddComponentMenu("Hidden/")]
    public class ActuatorAimer : MonoBehaviour
    {
        public Transform Target;

        void Update()
        {
            foreach (var endEffector in FindObjectsByType<EndEffector>(FindObjectsSortMode.InstanceID))
            {
                endEffector.AimAtPosition(Target.position, AdvancedTime.SmartDeltaTime);
            }
        }
    }
}