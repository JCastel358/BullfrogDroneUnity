using AdvancedTurrets.Libraries;
using AdvancedTurrets.Serialization;
using AdvancedTurrets.Utilities;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Spawners
{
    /// <summary>
    /// Spawner for <see cref="ParticleSystem"/> components with optional velocity inheritance.
    /// If no rigidbody is specified, velocity inheritance will be calculated manually and could be inconsistent.
    /// </summary>
    public class ParticleSpawner : BaseSpawner<ParticleSystem>
    {
        [Tooltip("If true, spawned particles will inherit velocity from their parent.")]
        public bool InheritVelocity = true;

        [Tooltip("Multiplier applied to the inherited velocity.")]
        public float VelocityMultiplier = 1f;

        [SerializeField]
        [Tooltip("The rigidbody that will be used to calculate particle velocity inheritance (if applicable).")]
        private LazyComponent<Rigidbody> _lazyRigidbody = new(ComponentAncestry.InParent);

        private Vector3 _lastVelocity;
        private Vector3 _lastPosition;

        private void FixedUpdate()
        {
            if (InheritVelocity)
            {
                if (_lazyRigidbody.Get(this) is Rigidbody rigidbody)
                {
                    _lastVelocity = rigidbody.GetVelocity();
                }
                else
                {
                    var deltaTime = AdvancedTime.SmartDeltaTime;
                    var dP = transform.position - _lastPosition;
                    _lastVelocity = dP / deltaTime;
                    _lastPosition = transform.position;
                }
            }
        }

        protected override void OnSpawned(ParticleSystem particleSystem)
        {
            base.OnSpawned(particleSystem);
            ApplyInheritVelocity(particleSystem);
        }

        private void ApplyInheritVelocity(ParticleSystem particleSystem)
        {
            if (!InheritVelocity)
            {
                return;
            }

            var netVelocity = _lastVelocity * VelocityMultiplier;
            var velocityOverLifetime = particleSystem.velocityOverLifetime;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
            velocityOverLifetime.xMultiplier = netVelocity.x;
            velocityOverLifetime.yMultiplier = netVelocity.y;
            velocityOverLifetime.zMultiplier = netVelocity.z;
            velocityOverLifetime.enabled = true;
        }
    }
}
