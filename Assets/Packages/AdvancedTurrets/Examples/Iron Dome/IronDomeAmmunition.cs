using AdvancedTurrets.Behaviours;
using AdvancedTurrets.Behaviours.Ammunitions;
using AdvancedTurrets.Serialization;
using UnityEngine;

namespace AdvancedTurrets.Examples.IronDome
{
    [AddComponentMenu("Hidden/")]
    [RequireComponent(typeof(BaseAmmunition))]
    public class IronDomeAmmunition : InstancedBehaviour<IronDomeAmmunition>
    {
        [SerializeField] LazyComponent<BaseAmmunition> _lazyAmmunition = new(ComponentAncestry.OnSelf);
        public BaseAmmunition Ammunition => _lazyAmmunition.Get(this);

        public Alignments Alignment;
        public IronDomeAmmunition Target;
        public bool Intercepted;

        public void Initialize(Alignments alignment, IronDomeAmmunition target = default)
        {
            Alignment = alignment;
            Target = target;
            Intercepted = false;
        }
    }
}