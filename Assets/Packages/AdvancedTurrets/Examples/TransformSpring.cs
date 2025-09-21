using AdvancedTurrets.Utilities;
using UnityEngine;

namespace AdvancedTurrets.Examples
{
    [AddComponentMenu("Hidden/")]
    public class TransformSpring : MonoBehaviour
    {
        public Vector3 Force;

        public Vector3 Center;
        Vector3 _velocity;

        void Update()
        {
            var error = transform.position - Center;

            var xForce = Mathf.Sign(error.x) * Force.x;
            var yForce = Mathf.Sign(error.y) * Force.y;
            var zForce = Mathf.Sign(error.z) * Force.z;

            _velocity -= new Vector3(xForce, yForce, zForce) * AdvancedTime.SmartDeltaTime;
            transform.Translate(_velocity * AdvancedTime.SmartDeltaTime);
        }
    }
}