using AdvancedTurrets.Libraries;
using AdvancedTurrets.Kinematics;
using UnityEditor;
using UnityEngine;
using AdvancedTurrets.Behaviours.Turrets;

namespace AdvancedTurrets.Editor
{
    [CustomEditor(typeof(TargetingModule)), CanEditMultipleObjects]
    public class TargetingModuleEditor : AdvancedEditor<TargetingModule>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            AddListener(tm => tm.Component, OnComponentChanged);
            AddListener(tm => tm.Rigidbody, OnRigidbodyChanged);
            AddListener(tm => tm.IKinematicGameObject, OnIKinematicGameObjectChanged);
        }

        void OnComponentChanged(Component from, Component to)
        {
            if (to)
            {
                TTargets.ForEach(tm => tm.Type = TargetingModule.TargetType.Component);
                ClearOldReferences();
            }
        }

        void OnRigidbodyChanged(Rigidbody from, Rigidbody to)
        {
            if (to)
            {
                TTargets.ForEach(tm => tm.Type = TargetingModule.TargetType.Rigidbody);
                ClearOldReferences();
            }
        }

        void OnIKinematicGameObjectChanged(GameObject from, GameObject to)
        {
            if (to)
            {
                TTargets.ForEach(tm => tm.Type = TargetingModule.TargetType.IKinematic);
                ClearOldReferences();

                if (to.GetComponent<IKinematic>() == default)
                {
                    Debug.LogWarning($"Unable to find {nameof(IKinematic)} on {to}");
                }
            }
        }

        void ClearOldReferences()
        {
            TTargets.ForEach(tm =>
            {
                if (tm.Type != TargetingModule.TargetType.Component)
                {
                    tm.Component = default;
                }

                if (tm.Type != TargetingModule.TargetType.Rigidbody)
                {
                    tm.Rigidbody = default;
                }

                if (tm.Type != TargetingModule.TargetType.IKinematic)
                {
                    tm.IKinematicGameObject = default;
                }
            });
        }
    }
}