using System.Collections.Generic;
using AdvancedTurrets.Behaviours.Collisions;
using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;


namespace AdvancedTurrets.Examples.ClayPigeons
{
    [AddComponentMenu("Hidden/")]
    [RequireComponent(typeof(DynamicColliderGroup))]
    public class ClayPigeonTargetController : MonoBehaviour
    {
        readonly List<ClayPigeon> clayPigeons = new();

        readonly HashSet<MuzzleLoader> _muzzleLoaders = new();

        DynamicColliderGroup _dynamicColliderGroup;

        void Awake()
        {
            _dynamicColliderGroup = GetComponent<DynamicColliderGroup>();
            clayPigeons.Clear();
        }

        void Start()
        {
            ClayPigeon.AnyClayPigeonLaunched += OnAnyClayPigeonLaunched;
        }

        void OnAnyClayPigeonLaunched(ClayPigeon clayPigeon)
        {
            clayPigeons.Insert(0, clayPigeon);
        }

        void Update()
        {
            foreach (var turret in FindObjectsByType<BaseTurret>(FindObjectsSortMode.InstanceID))
            {
                if (_muzzleLoaders.Add(turret.MuzzleLoader))
                {
                    // Make sure all assigned turrets drop their target each time they fire
                    turret.MuzzleLoader.AnyMuzzleFired.AddListener(m => turret.TargetingModule.ClearTarget());
                    if (turret is KinematicTurret kinematicTurret)
                    {
                        kinematicTurret.AmmunitionFired.AddListener(_dynamicColliderGroup.IgnoreCollisions);
                    }
                }

                // Verify that turrets can hit their assigned target if they already have one
                if (turret.TargetingModule.GetTargetTrajectory(out var target))
                {
                    if (turret is KinematicTurret kinematicTurret)
                    {
                        var fromPosition = kinematicTurret.MuzzleLoader.PeekNextMuzzle().transform.position;
                        if (!kinematicTurret.Intercept(out _, target, fromPosition, kinematicTurret.Spread, 0))
                        {
                            turret.TargetingModule.ClearTarget();
                        }
                    }
                }

                if (!turret.TargetingModule.HasTarget())
                {
                    for (int i = clayPigeons.Count - 1; i >= 0; i--)
                    {
                        var clayPigeon = clayPigeons[i];

                        if (turret is KinematicTurret kinematicTurret)
                        {
                            if (CanKinematicTurretIntercept(kinematicTurret, clayPigeon))
                            {
                                kinematicTurret.TargetingModule.SetTarget(clayPigeon.Projectile);
                                clayPigeons.RemoveAt(i);
                                break;
                            }
                        }
                        else
                        {
                            turret.TargetingModule.SetTarget(clayPigeon);
                            clayPigeons.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            clayPigeons.RemoveAll(cp => !cp.gameObject.activeSelf);
        }

        static bool CanKinematicTurretIntercept(KinematicTurret kinematicTurret, ClayPigeon clayPigeon)
        {
            var target = clayPigeon.Projectile.GetTrajectory();
            var fromPosition = kinematicTurret.MuzzleLoader.PeekNextMuzzle().transform.position;
            return kinematicTurret.Intercept(out _, target, fromPosition, kinematicTurret.Spread, 0);
        }
    }
}