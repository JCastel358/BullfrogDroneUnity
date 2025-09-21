using System.Collections.Generic;
using AdvancedTurrets.Behaviours.Turrets;
using AdvancedTurrets.Geometrics;
using UnityEngine;
using UnityEngine.Events;


namespace AdvancedTurrets.Behaviours.Ammunitions
{
    /// <summary>
    /// The base class for beam ammunition (hit scan) used in <see cref="BeamTurret">.
    /// Triggers RaycastHit events when the beam interacts with objects in the scene.
    /// </summary>
    /// <remarks>
    /// Derived classes can override <see cref="Fire(Line, IEnumerable{RaycastHit})"/> to customize behavior.
    /// </remarks>
    public class BaseBeam : MonoBehaviour
    {
        /// <summary>
        /// Fires the beam along the specified line and processes broadcasts the associated raycast hit results.
        /// </summary>
        public virtual void Fire(Line line, IEnumerable<RaycastHit> raycastHits)
        {
            foreach (var raycastHit in raycastHits)
            {
                OnRaycastHit.Invoke(raycastHit);
            }
        }

        [Tooltip("Invoked when the beam collides with a collider.")]
        public UnityEvent<RaycastHit> OnRaycastHit = new();
    }
}
