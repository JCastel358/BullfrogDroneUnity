using UnityEngine;
using UnityEngine.Events;

namespace AdvancedTurrets.Behaviours.Collisions
{
    /// <summary>
    /// Relays collision callbacks to a UnityEvent.
    /// This allows external components to customize collision logic without direct coding.
    /// </summary>
    [DisallowMultipleComponent]
    public class CollisionRelay : MonoBehaviour
    {
        [Tooltip("Invoked when this object collides with another. Provides collision details.")]
        public UnityEvent<Collision> Collision = new();

        private void OnCollisionEnter(Collision collision)
        {
            Collision.Invoke(collision);
        }
    }
}
