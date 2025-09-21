using AdvancedTurrets.Behaviours.Turrets;
using AdvancedTurrets.Utilities;
using UnityEngine;

namespace AdvancedTurrets.Examples.Randomization
{
    [AddComponentMenu("Hidden/")]
    public class RandomizationDetail : ExampleGUI<MuzzleLoader>
    {
        int _shotsFired;
        float _startedTime;
        private void Start()
        {
            foreach (var muzzle in TTarget.Muzzles)
            {
                muzzle.Fired.AddListener(tError => _shotsFired++);
            }

            _startedTime = AdvancedTime.Time;
        }

        protected override string GetText(MuzzleLoader t)
        {
            var result = $"{nameof(t.RandomizePercentage)}={t.RandomizePercentage * 100:N2}%";
            result += $"\nNominal {nameof(t.FireRate)}={t.FireRate}";

            var elapsedTime = AdvancedTime.Time - _startedTime;
            var shotsPerSecond = _shotsFired / elapsedTime;

            result += $"\nMeasured {nameof(t.FireRate)}={shotsPerSecond:N2}";
            result += $"\nError={(shotsPerSecond - t.FireRate) / t.FireRate * 100:N2}%";

            return result;
        }
    }
}