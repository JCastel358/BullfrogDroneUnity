using System.Collections.Generic;
using AdvancedTurrets.Libraries;
using UnityEngine;

namespace AdvancedTurrets.Behaviours.Collisions
{
    /// <summary>
    /// Toggles collisions between all colliders added to this group during runtime.
    /// Useful for toggling collisions between projectiles that are on the same team.
    /// </summary>
    public class DynamicColliderGroup : BaseColliderGroup
    {
        private readonly List<Collider> _dynamicColliders = new();

        public override IEnumerable<Collider> GetColliders() => _dynamicColliders;

        public override void IgnoreCollisions(Collider collider)
        {
            base.IgnoreCollisions(collider);
            _dynamicColliders.Add(collider);
        }

        public override void AllowCollisions(Collider collider)
        {
            _dynamicColliders.Remove(collider);
            base.AllowCollisions(collider);
        }

        private void LateUpdate()
        {
            _dynamicColliders.RemoveWhere(c => !c);
        }
    }
}
