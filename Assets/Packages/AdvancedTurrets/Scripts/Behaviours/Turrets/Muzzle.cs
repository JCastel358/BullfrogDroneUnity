using AdvancedTurrets.Behaviours.Actuators;
using UnityEngine;
using UnityEngine.Events;


namespace AdvancedTurrets.Behaviours.Turrets
{
    /// <summary>
    /// Muzzles serve as designated points where projectiles originate. inheriting from <see cref="EndEffector"/>.
    /// While all Muzzles are EndEffectors, not all EndEffectors are Muzzles.
    /// Some EndEffectors may be used solely for aiming, whereas Muzzles are always for firing from.
    /// </summary
    public class Muzzle : EndEffector
    {
        public void Fire(float tError)
        {
            Fired.Invoke(tError);
        }

        [Tooltip("Invoked when this muzzle fires and includes the continuous time error if appplicable.")]
        public UnityEvent<float> Fired = new();
    }
}
