using UnityEngine;

namespace AdvancedTurrets.Examples.Juggling
{
    [AddComponentMenu("Hidden/")]
    public class JugglingDetail : ExampleGUI<Transform>
    {
        float _maxHeight = float.NegativeInfinity;
        private void Update()
        {
            _maxHeight = Mathf.Max(TTarget.position.y, _maxHeight);
        }

        int _collisions = 0;
        void OnCollisionEnter(Collision collision)
        {
            _collisions++;
        }

        protected override string GetText(Transform t)
        {
            var result = "";
            result += $"Max Height: {_maxHeight:N2}m";
            result += $"\nCollisions: {_collisions}";
            return result;
        }
    }
}