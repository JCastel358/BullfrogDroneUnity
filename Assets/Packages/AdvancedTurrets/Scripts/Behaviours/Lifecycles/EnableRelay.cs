using UnityEngine;
using UnityEngine.Events;


namespace AdvancedTurrets.Behaviours.Lifecycles
{
    /// <summary>
    /// Relays all <see cref="OnEnable"/> events and broadcasts them as a UnityEvent.
    /// This allows external components to react when the GameObject or script is enabled.
    /// </summary>
    public class EnableRelay : MonoBehaviour
    {
        private void OnEnable()
        {
            Enabled.Invoke();
        }

        [Tooltip("Invoked when this script or GameObject is enabled.")]
        public UnityEvent Enabled = new();
    }
}
