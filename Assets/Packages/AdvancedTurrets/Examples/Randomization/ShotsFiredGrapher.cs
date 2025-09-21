using AdvancedTurrets.Utilities;
using AdvancedTurrets.Behaviours.Turrets;
using UnityEditor;
using UnityEngine;

namespace AdvancedTurrets.Examples.Randomization
{
    [AddComponentMenu("Hidden/")]
    public class ShotsFiredGrapher : MonoBehaviour
    {
        BaseTurret Turret => GetComponentInParent<BaseTurret>();

        public float XScalar = 0.1f;
        public float YScalar = 0.1f;

        Vector3 _lp0;
        private void Start()
        {
            foreach (var m in Turret.MuzzleLoader.Muzzles)
            {
                m.Fired.AddListener(OnShotFired);
            }

            _lp0 = transform.localPosition;
        }

        int _shots;
        void OnShotFired(float tError)
        {
            _shots += 1;
        }

        float x => AdvancedTime.Time * XScalar;
        float y => _shots / AdvancedTime.Time * YScalar;

        private void Update()
        {
            if (AdvancedTime.Time > 0)
            {
                transform.position = transform.parent.TransformPoint(_lp0) + Vector3.right * x + Vector3.up * y;
            }
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying && AdvancedTime.Time > 0)
            {
                Handles.Label(transform.parent.TransformPoint(_lp0) + new Vector3(x, y - 1), $"{_shots / AdvancedTime.Time:N4}");
            }
        }
    }
}