using System.Collections.Generic;
using AdvancedTurrets.Behaviours.Lifecycles;
using UnityEngine;


namespace AdvancedTurrets.Examples.ClayPigeons
{
    // Beams won't trigger destruction of clay pigeons by default since there's no collision
    // This script will directly destroy any raycast hit clay pigeons
    [AddComponentMenu("Hidden/")]
    public class ClayPigeonBeamDestroyer : MonoBehaviour
    {
        public void DestroyRaycastHitClayPigeon(IEnumerable<RaycastHit> raycastHits)
        {
            foreach (var raycastHit in raycastHits)
            {
                if (raycastHit.collider.GetComponentInParent<ClayPigeon>() is ClayPigeon clayPigeon)
                {
                    PoolingService.Instance.PoolOrDestroy(clayPigeon);
                }
            }
        }
    }
}