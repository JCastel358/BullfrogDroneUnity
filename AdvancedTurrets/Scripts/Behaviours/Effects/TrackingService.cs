using System.Collections.Generic;
using AdvancedTurrets.Libraries;
using UnityEngine;

namespace AdvancedTurrets.Behaviours.Effects
{
    /// <summary>
    /// A singleton service that allows one transform to follow another.
    /// Ensures smooth position and rotation tracking between objects.
    /// </summary>
    [DefaultExecutionOrder(int.MaxValue)]
    public class TrackingService : SingletonBehaviour<TrackingService>
    {
        protected override bool DestroyOldInstanceOnDuplication => false;

        private readonly Dictionary<Transform, Transform> _tFollowCache = new();

        /// <summary>
        /// Assigns a transform to follow another transform which will be managed by this class.
        /// </summary>
        public void Follow(Transform transform, Transform follow)
        {
            if (transform != null && follow != null)
            {
                _tFollowCache[transform] = follow;
            }
        }

        /// <summary>
        /// Clears the follower of the given transform.
        /// </summary>
        public void Unfollow(Transform transform)
        {
            _tFollowCache.Remove(transform);
        }

        /// <summary>
        /// Updates the position and rotation of all tracked transforms each frame.
        /// Removes entries if either the follower or target is inactive or destroyed.
        /// </summary>
        private void Update()
        {
            _tFollowCache.RemoveWhere(entry =>
            {
                var follower = entry.Key;
                var target = entry.Value;

                // Won't update position/rotation if either object is null or the following object is diabled.
                if (follower == null || !follower.gameObject.activeInHierarchy || target == null)
                {
                    return true;
                }

                // Update position and rotation
                target.GetPositionAndRotation(out var position, out var rotation);
                follower.SetPositionAndRotation(position, rotation);

                // If the target is inactive, following will not continue.
                if (!target.gameObject.activeInHierarchy)
                {
                    return true;
                }

                return false;
            });
        }
    }
}