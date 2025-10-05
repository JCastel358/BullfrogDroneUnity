using AdvancedTurrets.Behaviours;
using AdvancedTurrets.Behaviours.Lifecycles;
using UnityEngine;

namespace AdvancedTurrets.Examples.IronDome
{
    [AddComponentMenu("Hidden/")]
    [RequireComponent(typeof(AdvancedHealth))]
    public class IronDomeTarget : InstancedBehaviour<IronDomeTarget>
    {
        public Collider Collider { get; private set; }

        private void Awake()
        {
            Collider = GetComponentInChildren<Collider>();
        }

        public Vector3 GetRandomPointInCollider()
        {
            var bounds = Collider.ClosestPointOnBounds(transform.position + Random.onUnitSphere * 1000);
            return transform.position + (bounds - transform.position) * Random.Range(0, 1f);
        }
    }
}