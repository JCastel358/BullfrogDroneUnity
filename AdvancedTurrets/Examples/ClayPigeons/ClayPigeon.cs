using System;
using AdvancedTurrets.Behaviours.Ammunitions;
using AdvancedTurrets.Serialization;
using UnityEngine;

namespace AdvancedTurrets.Examples.ClayPigeons
{
    [AddComponentMenu("Hidden/")]
    [RequireComponent(typeof(Projectile))]
    public class ClayPigeon : MonoBehaviour
    {
        [SerializeField] LazyComponent<Projectile> _lazyProjectile = new(ComponentAncestry.OnSelf);
        public Projectile Projectile => _lazyProjectile.Get(this);

        void Awake()
        {
            Projectile.TrajectoryLaunched.AddListener(p => AnyClayPigeonLaunched?.Invoke(this));
        }

        public static event Action<ClayPigeon> AnyClayPigeonLaunched;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ClearStaticResources()
        {
            AnyClayPigeonLaunched = default;
        }
    }
}