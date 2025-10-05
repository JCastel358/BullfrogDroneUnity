using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Libraries;
using AdvancedTurrets.Serialization;
using UnityEngine;

namespace AdvancedTurrets.Examples.TargetTypes
{
    [AddComponentMenu("Hidden/")]
    public class KinematicTarget : MonoBehaviour, IKinematic
    {
        [SerializeField]
        LazyComponent<Rigidbody> _lazyRigidbody = new(ComponentAncestry.OnSelf);
        public Rigidbody Rigidbody => _lazyRigidbody.Get(this);

        public Vector3 InitialVelocity;
        public bool InitialVelocityRelative;
        private void Start()
        {
            var initialVelocity = InitialVelocityRelative ? transform.TransformDirection(InitialVelocity) : InitialVelocity;
            Rigidbody.SetVelocity(initialVelocity);
        }

        public Vector3 Acceleration;
        public bool AccelerationRelative;
        private Vector3 GetAcceleration()
        {
            return AccelerationRelative ? transform.TransformDirection(Acceleration) : Acceleration;
        }

        private void FixedUpdate()
        {
            Rigidbody.AddForce(GetAcceleration(), ForceMode.Acceleration);
        }

        public Trajectory GetTrajectory(bool instantiatedOrEnabled = false, float duration = default)
        {
            return new Trajectory(transform.position, Rigidbody.GetVelocity(), Rigidbody.useGravity, GetAcceleration(), duration, instantiatedOrEnabled, transform);
        }
    }
}