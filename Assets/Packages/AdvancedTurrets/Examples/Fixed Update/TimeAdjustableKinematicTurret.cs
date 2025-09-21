using UnityEngine;
using AdvancedTurrets.Behaviours.Turrets;

namespace AdvancedTurrets.Examples.FixedUpdate
{
    public enum UpdateTimes
    {
        Update,
        FixedUpdate
    }

    [AddComponentMenu("Hidden/")]
    public class TimeAdjustableKinematicTurret : KinematicTurret
    {
        public UpdateTimes UpdateTime;

        protected override void Update()
        {
            if (UpdateTime == UpdateTimes.Update)
            {
                base.Update();
            }
        }

        void FixedUpdate()
        {
            if (UpdateTime == UpdateTimes.FixedUpdate)
            {
                base.Update();
            }
        }
    }
}