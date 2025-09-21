using AdvancedTurrets.Serialization;
using UnityEditor;
using UnityEngine;


namespace AdvancedTurrets.Examples
{
    public abstract class ExampleGUI<T> : MonoBehaviour where T : Component
    {
        [SerializeField] LazyComponent<T> _lazyTTarget = new(ComponentAncestry.InParent);
        protected T TTarget => _lazyTTarget.Get(this);

        void OnDrawGizmos()
        {
            if (TTarget == default)
            {
                return;
            }

            if (!Application.isPlaying)
            {
                return;
            }

            var text = GetText(TTarget);
            Handles.Label(transform.position, text, new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                normal = new()
                {
                    textColor = Color.white
                },
            });
        }

        protected abstract string GetText(T t);
    }
}