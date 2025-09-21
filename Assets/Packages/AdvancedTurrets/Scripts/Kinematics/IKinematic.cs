
namespace AdvancedTurrets.Kinematics
{
    /// <summary>
    /// Defines an interface for objects that can provide their own trajectory information.
    /// This is used by the <see cref="Behaviours.Modules.TargetingModule"/> to determine the movement of objects.
    /// </summary>
    public interface IKinematic
    {
        /// <summary>
        /// Retrieves the trajectory of the implementation.
        /// </summary>
        /// <remarks>
        /// If an <see cref="IKinematic"/> was created and immediately thereafter its trajectory is measured, <paramref name="instantiatedOrEnabled"/> is true.
        /// This holds true in both Update() and FixedUpdate() and should be set accordingly.
        /// </remarks>
        /// <param name="instantiatedOrEnabled">Whether or not this object was instantiated or enabled within the same time frame that this is invoked</param>
        /// <param name="duration">Duration of the trajectory. This really only matters for gizmos or any custom implementations that might reference it.</param>
        /// <returns>The computed <see cref="Trajectory"/> of the object.</returns>
        public Trajectory GetTrajectory(bool instantiatedOrEnabled = false, float duration = default);
    }
}
