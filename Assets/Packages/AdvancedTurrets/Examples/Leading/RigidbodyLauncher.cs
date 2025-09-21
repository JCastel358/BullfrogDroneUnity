using AdvancedTurrets.Libraries;
using AdvancedTurrets.Serialization;
using UnityEngine;

namespace AdvancedTurrets.Examples.Leading
{
    [AddComponentMenu("Hidden/")]
    public class RigidbodyLauncher : MonoBehaviour
    {
        [SerializeField]
        LazyComponent<Rigidbody> _lazyRigidbody = new(ComponentAncestry.InParent);
        public Rigidbody Rigidbody => _lazyRigidbody.Get(this);

        public Vector3 Velocity;
        public bool VelocityIsRelative;
        public Vector3 AngularVelocity;
        public bool AngularVelocityIsRelative;

        void Start()
        {
            Rigidbody.SetVelocity(VelocityIsRelative ? transform.TransformDirection(Velocity) : Velocity);
            Rigidbody.angularVelocity = AngularVelocityIsRelative ? transform.TransformDirection(AngularVelocity) : AngularVelocity;
            Destroy(this);
        }
    }
}