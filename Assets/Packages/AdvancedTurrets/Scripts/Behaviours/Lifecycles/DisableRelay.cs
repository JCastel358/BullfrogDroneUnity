using UnityEngine;
using UnityEngine.Events;


namespace AdvancedTurrets.Behaviours.Lifecycles
{
    /// <summary>
    /// Relays all <see cref="OnDisable"/> callbacks and broadcasts them as an event.
    /// This allows external components to respond to different types of disable events.
    /// </summary>
    public class DisableRelay : MonoBehaviour
    {
        [Tooltip("If enabled, the Disabled event is raised when this script is disabled.")]
        public bool ThisScriptDisabled = true;

        [Tooltip("If enabled, the Disabled event is raised when this GameObject is set inactive.")]
        public bool ThisGameObjectDisabled = true;

        [Tooltip("If enabled, the Disabled event is raised when a parent GameObject is set inactive.")]
        public bool ParentGameObjectDisabled = true;

        protected virtual void OnDisable()
        {
            if (!enabled)
            {
                if (ThisScriptDisabled)
                {
                    RaiseDisabled();
                }
            }
            else if (gameObject.activeSelf)
            {
                if (ParentGameObjectDisabled)
                {
                    RaiseDisabled();
                }
            }
            else if (ThisGameObjectDisabled)
            {
                RaiseDisabled();
            }
        }

        private void RaiseDisabled()
        {
            if (!gameObject.scene.isLoaded)
            {
                return;
            }

            Disabled.Invoke();
        }

        [Tooltip("Invoked when script determines it has been disabled depending on its parameters.")]
        public UnityEvent Disabled = new();
    }
}
