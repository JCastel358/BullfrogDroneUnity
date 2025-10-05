using System.Collections.Generic;
using AdvancedTurrets.Behaviours.Lifecycles;
using AdvancedTurrets.Libraries;
using UnityEngine;

namespace AdvancedTurrets.Examples.Health
{
    [AddComponentMenu("Hidden/")]
    public class HealthDamage : MonoBehaviour
    {
        public float Damage = 10;

        AdvancedHealth _sender;
        public void SetSender(AdvancedHealth sender)
        {
            _sender = sender;
        }

        public void ApplyDamage(GameObject gameObject)
        {
            if (gameObject.GetComponentInParent<AdvancedHealth>() is AdvancedHealth health)
            {
                health.TakeDamage(Damage, out _, _sender);
            }
        }

        // Just various overloads which can be hit via unity events
        public void ApplyDamage(Component component) => ApplyDamage(component.gameObject);
        public void ApplyDamage(Collision collision) => ApplyDamage(collision.gameObject);
        public void ApplyDamage(RaycastHit raycastHit) => ApplyDamage(raycastHit.collider.gameObject);
        public void ApplyDamage(IEnumerable<RaycastHit> raycastHits) => raycastHits.ForEach(rch => ApplyDamage(rch));
    }
}