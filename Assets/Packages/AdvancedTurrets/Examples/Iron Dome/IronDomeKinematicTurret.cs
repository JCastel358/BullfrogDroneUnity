using AdvancedTurrets.Behaviours.Ammunitions;
using AdvancedTurrets.Libraries;
using AdvancedTurrets.Behaviours.Turrets;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using AdvancedTurrets.Serialization;
using AdvancedTurrets.Mathematics;

namespace AdvancedTurrets.Examples.IronDome
{
    public enum Alignments
    {
        Defender,
        Attacker
    }

    public enum AttackerTargetingPreferences
    {
        Random, //targets chosen randomly
        Furthest, //Prefers far targets
        Closest, //Preferes closer targets
    }

    public enum DefenderTargetingTypes
    {
        ClosestToSights, //prefer targets that are closest to sights
        AntiAir, //prefer shooting the highest targets
        Smart, //prefer shooting the targets with least time remaining
        Random, //No preference, just take something random each time
    }

    public enum TargetSwitchingPreferences
    {
        AfterTimeOut,
        AfterShotFired,
        AfterCycle,
    }

    [AddComponentMenu("Hidden/")]
    [RequireComponent(typeof(KinematicTurret))]
    public class IronDomeKinematicTurret : MonoBehaviour
    {
        public Alignments Alignment = Alignments.Defender;
        public AttackerTargetingPreferences AttackerPreference = AttackerTargetingPreferences.Closest;
        public DefenderTargetingTypes DefenderPreference = DefenderTargetingTypes.ClosestToSights;
        public TargetSwitchingPreferences TargetSwitchingPreference = TargetSwitchingPreferences.AfterShotFired;

        [SerializeField] LazyComponent<KinematicTurret> _lazyKinematicTurret = new(ComponentAncestry.InParent);
        public KinematicTurret KinematicTurret => _lazyKinematicTurret.Get(this);

        void Start()
        {
            KinematicTurret.AmmunitionFired.AddListener(OnAmmunitionFired);
            KinematicTurret.MuzzleLoader.ReloadStarted.AddListener(OnReloadStarted);
        }

        void Update()
        {
            // If a defender turret is intercepting and it is no longer feasible, release the target
            if (Alignment == Alignments.Defender)
            {
                if (DefenderPreference == DefenderTargetingTypes.Smart)
                {
                    if (KinematicTurret.TargetingModule.GetComponentOnTarget<IronDomeAmmunition>() is IronDomeAmmunition ida)
                    {
                        var timeRemaining = ida.Ammunition.Trajectory.GetRemainingTime();
                        var fromPosition = KinematicTurret.MuzzleLoader.PeekNextMuzzle().Position;
                        if (!KinematicTurret.Intercept(out var intercept, ida.Ammunition.GetTrajectory(), fromPosition) || intercept.Duration > timeRemaining)
                        {
                            KinematicTurret.TargetingModule.ClearTarget();
                        }
                    }
                }
            }

            // If there isn't a target or timeout has elapsed
            if (!KinematicTurret.TargetingModule.HasTarget() ||
                TargetSwitchingPreference == TargetSwitchingPreferences.AfterTimeOut && KinematicTurret.TimeoutClock.HasElapsed())
            {
                if (AcquireTarget())
                {
                    KinematicTurret.TimeoutClock.Reset();
                }
            }
        }

        bool AcquireTarget()
        {
            switch (Alignment)
            {
                case Alignments.Defender:
                    switch (DefenderPreference)
                    {
                        case DefenderTargetingTypes.ClosestToSights:
                            if (GetDefenderClosestToSights(out var ironDomeAmmoTag))
                            {
                                ironDomeAmmoTag.Intercepted = true;
                                KinematicTurret.TargetingModule.SetTarget(ironDomeAmmoTag.Ammunition);
                                return true;
                            }
                            break;
                        case DefenderTargetingTypes.AntiAir:
                            if (GetDefenderAntiAir(out ironDomeAmmoTag))
                            {
                                ironDomeAmmoTag.Intercepted = true;
                                KinematicTurret.TargetingModule.SetTarget(ironDomeAmmoTag.Ammunition);
                                return true;
                            }
                            break;
                        case DefenderTargetingTypes.Smart:
                            if (GetDefenderSmart(out ironDomeAmmoTag))
                            {
                                ironDomeAmmoTag.Intercepted = true;
                                KinematicTurret.TargetingModule.SetTarget(ironDomeAmmoTag.Ammunition);
                                return true;
                            }
                            break;
                        case DefenderTargetingTypes.Random:
                            if (GetDefenderRandomTarget(out ironDomeAmmoTag))
                            {
                                ironDomeAmmoTag.Intercepted = true;
                                KinematicTurret.TargetingModule.SetTarget(ironDomeAmmoTag.Ammunition);
                                return true;
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case Alignments.Attacker:
                    switch (AttackerPreference)
                    {
                        case AttackerTargetingPreferences.Random:
                            if (GetAttackerRandomTarget(out var ironDomeTarget))
                            {
                                KinematicTurret.TargetingModule.SetTarget(ironDomeTarget.GetRandomPointInCollider());
                                return true;
                            }
                            break;
                        case AttackerTargetingPreferences.Furthest:
                            if (GetAttackerFurthestTarget(out ironDomeTarget))
                            {
                                KinematicTurret.TargetingModule.SetTarget(ironDomeTarget.GetRandomPointInCollider());
                                return true;
                            }
                            break;
                        case AttackerTargetingPreferences.Closest:
                            if (GetAttackerClosestTarget(out ironDomeTarget))
                            {
                                KinematicTurret.TargetingModule.SetTarget(ironDomeTarget.GetRandomPointInCollider());
                                return true;
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
            }

            return false;
        }

        # region Defender Target Acquisitions

        bool GetDefenderSmart(out IronDomeAmmunition ironDomeAmmoTag)
        {
            Assert.IsTrue(KinematicTurret.MuzzleLoader.GetNextEndEffector(out var ef));

            var fromPosition = KinematicTurret.MuzzleLoader.PeekNextMuzzle().Position;

            ironDomeAmmoTag = IronDomeAmmunition.EnabledInstances
                .Where(ida => ida.Alignment != Alignment)
                .Where(ida => !ida.Intercepted)
                .OrderBy(ida => ida.Ammunition.Trajectory.GetRemainingTime())
                .RankBy(ida => Vector3.Angle(ef.Forward, ef.Position + (ida.transform.position - ef.Position))) // how much do I have to turn
                .RankBy(ida =>
                {
                    if (!KinematicTurret.Intercept(out var intercept, ida.Ammunition.GetTrajectory(), fromPosition))
                    {
                        return float.PositiveInfinity;
                    }

                    return intercept.Duration; // how long will it take to intercept
                })
                .BestOrDefault()?.Key;

            return ironDomeAmmoTag;
        }

        bool GetDefenderAntiAir(out IronDomeAmmunition ironDomeAmmoTag)
        {
            ironDomeAmmoTag = IronDomeAmmunition.EnabledInstances
                .Where(ida => ida.Alignment != Alignment)
                .Where(ida => !ida.Intercepted)
                .OrderBy(ida => ida.transform.position.y)
                .LastOrDefault();

            return ironDomeAmmoTag;
        }

        bool GetDefenderClosestToSights(out IronDomeAmmunition ironDomeAmmoTag)
        {
            Assert.IsTrue(KinematicTurret.MuzzleLoader.GetNextEndEffector(out var endEffector));

            ironDomeAmmoTag = IronDomeAmmunition.EnabledInstances
                .Where(ida => ida.Alignment != Alignment)
                .Where(ida => !ida.Intercepted)
                .OrderBy(ida => Vector3.Angle(endEffector.Forward, endEffector.Position + (ida.transform.position - endEffector.Position)))
                .FirstOrDefault();

            return ironDomeAmmoTag;
        }

        bool GetDefenderRandomTarget(out IronDomeAmmunition ironDomeAmmoTag)
        {
            ironDomeAmmoTag = IronDomeAmmunition.EnabledInstances
                .Where(ida => ida.Alignment != Alignment)
                .Where(ida => !ida.Intercepted)
                .RandomOrDefault();

            return ironDomeAmmoTag;
        }

        #endregion

        # region Attacker Target Acquisitions

        bool GetAttackerClosestTarget(out IronDomeTarget ironDomeTarget)
        {
            ironDomeTarget = IronDomeTarget.EnabledInstances
                .OrderBy(idt => (idt.transform.position - transform.position).magnitude)
                .FirstOrDefault();

            return ironDomeTarget;
        }

        bool GetAttackerFurthestTarget(out IronDomeTarget ironDomeTarget)
        {
            ironDomeTarget = IronDomeTarget.EnabledInstances
                .OrderBy(idt => (idt.transform.position - transform.position).magnitude)
                .LastOrDefault();

            return ironDomeTarget;
        }

        bool GetAttackerRandomTarget(out IronDomeTarget ironDomeTarget)
        {
            ironDomeTarget = IronDomeTarget.EnabledInstances.RandomOrDefault();

            return ironDomeTarget;
        }

        #endregion

        void OnReloadStarted()
        {
            if (TargetSwitchingPreference == TargetSwitchingPreferences.AfterCycle)
            {
                KinematicTurret.TargetingModule.ClearTarget();
            }
        }

        void OnAmmunitionFired(BaseAmmunition ammunition)
        {
            if (ammunition.GetComponent<IronDomeAmmunition>() is not IronDomeAmmunition ironDomeAmmunition)
            {
                ironDomeAmmunition = ammunition.gameObject.AddComponent<IronDomeAmmunition>();
            }

            ironDomeAmmunition.Initialize(Alignment, KinematicTurret.TargetingModule.GetComponentOnTarget<IronDomeAmmunition>());

            if (TargetSwitchingPreference == TargetSwitchingPreferences.AfterShotFired)
            {
                KinematicTurret.TargetingModule.ClearTarget();
            }
        }
    }
}