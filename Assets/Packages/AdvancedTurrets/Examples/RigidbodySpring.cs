using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Libraries;
using AdvancedTurrets.Serialization;
using UnityEngine;


namespace AdvancedTurrets.Examples
{
    [AddComponentMenu("Hidden/")]
    public class RigidbodySpring : MonoBehaviour, IKinematic
    {
        [SerializeField] LazyComponent<Rigidbody> _lazyRigidbody = new(ComponentAncestry.OnSelf);
        public Rigidbody Rigidbody => _lazyRigidbody.Get(this);

        public Vector3 Center;
        public Vector3 Force = Vector3.one * 5;

        void FixedUpdate()
        {
            var force = GetForce();
            Rigidbody.AddForce(force, ForceMode.Acceleration);
        }

        Vector3 GetForce()
        {
            var error = transform.position - Center;
            var xForce = Mathf.Sign(error.x) * Force.x;
            var yForce = Mathf.Sign(error.y) * Force.y;
            var zForce = Mathf.Sign(error.z) * Force.z;
            var force = -new Vector3(xForce, yForce, zForce);
            return force;
        }

        public Trajectory GetTrajectory(bool instantiatedOrEnabled = false, float duration = 0)
        {
            return new Trajectory(transform.position, Rigidbody.GetVelocity(), Rigidbody.useGravity, GetForce());
        }
    }
}