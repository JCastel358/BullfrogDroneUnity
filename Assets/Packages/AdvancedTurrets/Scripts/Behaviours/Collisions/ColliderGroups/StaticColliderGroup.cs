using System.Collections.Generic;
using AdvancedTurrets.Serialization;
using UnityEngine;

namespace AdvancedTurrets.Behaviours.Collisions
{
    /// <summary>
    /// Manages collisions for a predefined set of colliders.
    /// Useful for toggling collision between a projectile and the turret/colliders it's being fired from.
    /// </summary>
    public class StaticColliderGroup : BaseColliderGroup
    {
        [SerializeField]
        [Tooltip("The colliders managed by this group. These will be used when toggling collisions with external colliders.")]
        private LazyComponents<Collider> _lazyColliders = new(ComponentAncestry.InChildren);

        public override IEnumerable<Collider> GetColliders() => _lazyColliders.Get(this);
    }
}
