using System.Collections.Generic;
using System.Linq;
using AdvancedTurrets.Utilities;
using AdvancedTurrets.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using AdvancedTurrets.Behaviours.Actuators;
using AdvancedTurrets.Serialization;


namespace AdvancedTurrets.Behaviours.Turrets
{
    /// <summary>
    /// Controls the core cycle timings, reload behaviours, and muzzle management for turrets.
    /// </summary>
    [DefaultExecutionOrder(0)]
    public class MuzzleLoader : MonoBehaviour
    {
        public enum MuzzleFireType
        {
            Incremental,
            Gattling,
            Randomized
        }

        [Tooltip("Whether or not this is reloaded and ready to fire at t=0. Otherwise, reload follows normal timing.")]
        public bool InstantReloadOnStart = true;

        [Tooltip("If false, the MuzzleLoader will fire when able to. Otherwise it will hold fire until this is enabled.")]
        public bool HoldFire = false;

        [Min(1)]
        [Tooltip("The number of times each muzzle can fire before needing to be reloaded.")]
        public int MuzzleCapacity = 1;

        [Min(0)]
        [Tooltip("Total shots fired per second across all muzzles.")]
        public float FireRate = 1;

        [Range(0, 1)]
        [Tooltip("0: All muzzles fire instantly, followed by a reload cycle. 1: Evenly spaced shots with zero reload time cycle at the end.")]
        public float BurstInterval = 1;

        [Range(0, 1)]
        [Tooltip("Adds randomness to fire/reload intervals to prevent unnatural patterns. This follows a normal distribution.")]
        public float RandomizePercentage = 0;

        [Tooltip("Indicates whether the MuzzleLoader is currently reloading.")]
        public bool Reloading = false;

        [Tooltip("If true, automatically reloads at the end of a cycle.")]
        public bool AutoReload = true;

        [Tooltip("Determines how muzzles are cycled through during firing.")]
        public MuzzleFireType FireType = MuzzleFireType.Incremental;

        [Tooltip("If true, projectiles may briefly appear behind the muzzle at the moment of firing.")]
        public bool ShiftBackward = false;

        [Tooltip("Applies a time shift to all shots fired by this MuzzleLoader.")]
        public float TimeShift = 0;

        [Tooltip("If specified, the turret will aim exclusively with this end effector and not aim with each muzzle individually.")]
        public EndEffector AimingEndEffector;

        [SerializeField]
        [Tooltip("The muzzles controlled by this MuzzleLoader.")]
        private LazyComponents<Muzzle> _lazyMuzzles = new(ComponentAncestry.InChildrenOrSiblingsChildren);
        public Muzzle[] Muzzles => _lazyMuzzles.Get(this);

        [Tooltip("The underlying limiter that enforces timing restrictions and calculates continuity errors.")]
        public RateLimiter RateLimiter = new();

        private readonly Queue<Muzzle> _muzzleQueue = new();

        public bool Loaded => _muzzleQueue.Count > 0;

        /// <summary>
        /// The end-to-end time spent firing all muzzles and then reloading
        /// </summary>
        public float CycleTime => FireRate == 0 ? 0 : Muzzles.Length * MuzzleCapacity / FireRate;

        /// <summary>
        /// The total time spent reloading
        /// </summary>
        public float ReloadTime => CycleTime * (1 - BurstInterval);

        /// <summary>
        /// The total time spent firing all muzzles
        /// </summary>
        public float BurstTime => CycleTime * BurstInterval / (Muzzles.Length * MuzzleCapacity);

        protected virtual void Start()
        {
            if (AutoReload)
            {
                Reload(InstantReloadOnStart);
            }
        }

        protected virtual void Update()
        {
            CheckReload();
        }

        /// <summary>
        /// This will be checked frequently at runtime and can both start or complete a reload cycle.
        /// </summary>
        private void CheckReload()
        {
            if (Reloading)
            {
                if (RateLimiter.CanPass)
                {
                    Reloading = false;
                    ReloadCompleted.Invoke();
                }
            }
            else if (AutoReload && !Loaded)
            {
                Reload();
            }
        }

        public bool CanFire()
        {
            if (HoldFire)
            {
                return false;
            }

            if (!RateLimiter.CanPass)
            {
                return false;
            }

            CheckReload();

            if (!Loaded)
            {
                return false;
            }

            if (FireRate <= 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Peeks whether or not a shot is ready and what it will be.
        /// </summary>
        public bool PeekNextShot(float deltaTime, out Shot shot)
        {
            if (!CanFire())
            {
                shot = default;
                return false;
            }

            var fixedShift = ShiftBackward ? -AdvancedTime.TimeSinceFixedUpdate : AdvancedTime.TimeUntilFixedUpdate;
            var tError = RateLimiter.GetContinuousError(deltaTime);
            var timeShift = tError + fixedShift + TimeShift;

            var muzzle = PeekNextMuzzle();
            shot = new Shot(muzzle, timeShift);
            return true;
        }

        /// <summary>
        /// Fires a shot that has already been peeked. This method should only be called after <see cref="PeekNextShot"/> has returned <c>true</c>.
        /// </summary>
        public bool FireShot(float deltaTime, Shot shot)
        {
            if (HoldFire)
            {
                return false;
            }

            var burstTime = BurstTime.Randomize(RandomizePercentage);
            RateLimiter.Pass(deltaTime, burstTime, out _);

            _muzzleQueue.Dequeue();
            shot.Muzzle.Fire(shot.TimeShift);
            AnyMuzzleFired.Invoke(shot.Muzzle);

            if (!Loaded)
            {
                Unloaded.Invoke();
                if (AutoReload)
                {
                    Reload();
                }
            }

            return true;
        }

        public bool Fire(float deltaTime, out Shot shot)
        {
            if (PeekNextShot(deltaTime, out shot))
            {
                BeforeFire.Invoke();
                return FireShot(deltaTime, shot);
            }

            return false;
        }

        /// <summary>
        /// Fires all available shots within the given time interval.
        /// This only happens when FireRate is faster than frame rate.
        /// </summary>
        public List<Shot> FireAll(float deltaTime)
        {
            var results = new List<Shot>();
            while (Fire(deltaTime, out var shot))
            {
                results.Add(shot);
            }
            return results;
        }

        /// <summary>
        /// Retrieves the next muzzle in the queue without removing it. If unloaded this will be null.
        /// </summary>
        public Muzzle PeekNextMuzzle()
        {
            if (_muzzleQueue.TryPeek(out var muzzle))
            {
                return muzzle;
            }

            return default;
        }

        /// <summary>
        /// Retrieves the end effector that will be used to aim the next shot with.
        /// </summary>
        public bool GetNextEndEffector(out EndEffector endEffector)
        {
            if (AimingEndEffector)
            {
                endEffector = AimingEndEffector;
                return true;
            }
            else if (PeekNextMuzzle() is Muzzle nextMuzzle)
            {
                endEffector = nextMuzzle;
                return true;
            }

            endEffector = default;
            return default;
        }

        /// <summary>
        /// Executes the reload process for the turret and resets its muzzle queue with new pointers.
        /// </summary>
        public void Reload(bool instantReload = false)
        {
            _muzzleQueue.Clear();
            if (!Muzzles.Any())
            {
                Debug.LogWarning($"Reloading {name} but no muzzles are assigned.");
            }

            switch (FireType)
            {
                case MuzzleFireType.Incremental:
                    for (var m = 0; m < Muzzles.Length; m++)
                    {
                        for (var i = 0; i < MuzzleCapacity; i++)
                        {
                            _muzzleQueue.Enqueue(Muzzles[m]);
                        }
                    }
                    break;
                case MuzzleFireType.Gattling:
                    for (var i = 0; i < MuzzleCapacity; i++)
                    {
                        for (var m = 0; m < Muzzles.Length; m++)
                        {
                            _muzzleQueue.Enqueue(Muzzles[m]);
                        }
                    }
                    break;
                case MuzzleFireType.Randomized:
                    foreach (var m in Enumerable.Range(0, Muzzles.Length).OrderBy(i => Random.value))
                    {
                        _muzzleQueue.Enqueue(Muzzles[m]);
                    }
                    break;
            }

            Reloading = !instantReload;
            if (instantReload)
            {
                RateLimiter.Reset(AdvancedTime.Time);
            }
            else
            {
                var reloadTime = ReloadTime.Randomize(RandomizePercentage);
                RateLimiter.AddWaitTime(reloadTime);
            }

            ReloadStarted.Invoke();
        }

        # region Events

        [Tooltip("Invoked when any muzzle managed by this fires.")]
        public UnityEvent<Muzzle> AnyMuzzleFired = new();

        [Tooltip("Invoked before firing a shot.")]
        public UnityEvent BeforeFire = new();

        [Tooltip("Invoked when the last shot in a given cycle is fired.")]
        public UnityEvent Unloaded = new();

        [Tooltip("Invoked when the reloading cycle starts.")]
        public UnityEvent ReloadStarted = new();

        [Tooltip("Invoked when the reloading cycle completes.")]
        public UnityEvent ReloadCompleted = new();

        # endregion
    }

    /// <summary>
    /// Represents a shot from a MuzzleLaoder, containing its originating muzzle and continuity time error.
    /// </summary>
    public struct Shot
    {
        public Muzzle Muzzle { get; }
        public float TimeShift { get; }

        public Shot(Muzzle muzzle, float timeShift)
        {
            Muzzle = muzzle;
            TimeShift = timeShift;
        }
    }
}