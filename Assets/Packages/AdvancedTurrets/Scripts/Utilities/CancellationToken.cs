
namespace AdvancedTurrets.Utilities
{
    /// <summary>
    /// A simple class used to handle cancellation requests.
    /// </summary>
    [System.Serializable]
    public class CancellationToken
    {
        private bool _isCancelled = false;

        public void RequestCancellation()
        {
            _isCancelled = true;
        }

        public bool IsCancellationRequested()
        {
            return _isCancelled;
        }
    }
}
