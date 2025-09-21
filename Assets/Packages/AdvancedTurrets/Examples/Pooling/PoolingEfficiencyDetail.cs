using AdvancedTurrets.Behaviours.Lifecycles;
using AdvancedTurrets.Behaviours.Turrets;
using UnityEngine;

namespace AdvancedTurrets.Examples.Pooling
{
    [AddComponentMenu("ATExamples/" + nameof(PoolingEfficiencyDetail))]
    public class PoolingEfficiencyDetail : ExampleGUI<KinematicTurret>
    {
        protected override string GetText(KinematicTurret t)
        {
            if (!Application.isPlaying)
            {
                return "";
            }

            var result = "";

            if (t.AmmunitionPrefab is MonoBehaviour ammunitionPrefab)
            {
                var pool = PoolingService.Instance.GetOrCreatePoolFromPrefab(ammunitionPrefab.gameObject);

                var efficiency = pool.Hits / Mathf.Max(pool.Hits + pool.Misses, 1f);

                result += $"{ammunitionPrefab.name}\n" +
                    $"{nameof(pool.ActiveCount)}={pool.ActiveCount}\n" +
                    $"{nameof(pool.InactiveCount)}={pool.InactiveCount}\n" +
                    $"{nameof(pool.TotalCount)}={pool.TotalCount}\n" +
                    $"{nameof(pool.Hits)}={pool.Hits}\n" +
                    $"{nameof(pool.Misses)}={pool.Misses}\n" +
                    $"Efficiency={efficiency * 100:N2}%";
            }

            return result;
        }
    }
}