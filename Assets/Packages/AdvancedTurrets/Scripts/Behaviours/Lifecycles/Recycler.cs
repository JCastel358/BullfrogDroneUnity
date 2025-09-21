using System.Collections;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Lifecycles
{
    /// <summary>
    /// Handles recycling or destruction of game objects.
    /// </summary>
    [DisallowMultipleComponent]
    public class Recycler : MonoBehaviour
    {
        [Tooltip("If true, the object will be returned to the pool if possible; otherwise, it will be destroyed.")]
        public bool RecycleToPool = true;

        /// <summary>
        /// Recycles the object by either pooling or destroying it.
        /// </summary>
        public void Recycle()
        {
            CancelRecycleAfterDelay();

            if (RecycleToPool)
            {
                PoolingService.Instance.PoolOrDestroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Recycles the object after a specified delay.
        /// </summary>
        public void RecycleAfterDelay(float delay)
        {
            if (delay > 0)
            {
                StartCoroutine(RecycleAfterDelayEnumerator(delay));
            }
            else
            {
                Recycle();
            }
        }

        /// <summary>
        /// Cancels recycling after a delay if applicable.
        /// </summary>
        public void CancelRecycleAfterDelay()
        {
            StopAllCoroutines();
        }

        private IEnumerator RecycleAfterDelayEnumerator(float delay)
        {
            yield return new WaitForSeconds(delay);
            Recycle();
        }
    }
}
