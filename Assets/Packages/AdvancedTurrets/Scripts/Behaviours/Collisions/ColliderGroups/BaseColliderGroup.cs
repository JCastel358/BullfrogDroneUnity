using AdvancedTurrets.Behaviours.Ammunitions;
using AdvancedTurrets.Libraries;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdvancedTurrets.Behaviours.Collisions
{
    /// <summary>
    /// Manages physics collision interactions between groups of colliders.
    /// Primarily used for collider Physics toggling (e.g. projectiles ignoring turret colliders, or friendly projectiles ignoring collisions with each other).
    /// </summary>
    public abstract class BaseColliderGroup : MonoBehaviour
    {
        /// <summary>
        /// The colliders managed by the group that will have their collisions toggled across all methods.
        /// </summary>
        public abstract IEnumerable<Collider> GetColliders();

        public void IgnoreCollisions(BaseColliderGroup colliderGroup)
        {
            IgnoreCollisions(colliderGroup.GetColliders());
        }

        public void IgnoreCollisions(BaseAmmunition ammunition)
        {
            IgnoreCollisions(ammunition.StaticColliderGroup.GetColliders());
        }

        public void IgnoreCollisions(IEnumerable<Collider> colliders)
        {
            colliders.ForEach(IgnoreCollisions);
        }

        public virtual void IgnoreCollisions(Collider collider)
        {
            GetColliders().Where(c => c).ForEach(c => Physics.IgnoreCollision(c, collider));
        }

        public void AllowCollisions(BaseColliderGroup colliderGroup)
        {
            AllowCollisions(colliderGroup.GetColliders());
        }

        public void AllowCollisions(BaseAmmunition ammunition)
        {
            AllowCollisions(ammunition.StaticColliderGroup.GetColliders());
        }

        public void AllowCollisions(IEnumerable<Collider> colliders)
        {
            colliders.ForEach(AllowCollisions);
        }

        public virtual void AllowCollisions(Collider collider)
        {
            GetColliders().Where(c => c).ForEach(c => Physics.IgnoreCollision(c, collider, false));
        }
    }
}
