using AdvancedTurrets.Utilities;
using UnityEngine;


namespace AdvancedTurrets.Examples
{
    [AddComponentMenu("Hidden/")]
    public class LookAtTarget : MonoBehaviour
    {
        public float LerpMagnitude = 1f;
        public Transform Target;

        void Update()
        {
            if (GetLookAtPosition(out var lookAtPosition))
            {
                var targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, AdvancedTime.SmartDeltaTime * LerpMagnitude);
            }
        }

        protected virtual bool GetLookAtPosition(out Vector3 position)
        {
            if (Target)
            {
                position = Target.position;
                return true;
            }

            position = default;
            return false;
        }
    }
}