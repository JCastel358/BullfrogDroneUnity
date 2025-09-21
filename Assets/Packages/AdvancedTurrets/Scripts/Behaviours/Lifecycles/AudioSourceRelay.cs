using AdvancedTurrets.Serialization;
using UnityEngine;
using UnityEngine.Events;


namespace AdvancedTurrets.Behaviours.Lifecycles
{
    /// <summary>
    /// Plays an audio clip with randomized pitch and volume, invoking an event when playback ends.
    /// </summary>
    public class AudioSourceRelay : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The AudioSource used to play audio clips.")]
        private LazyComponent<AudioSource> _lazyAudioSource = new(ComponentAncestry.OnSelf);
        public AudioSource AudioSource => _lazyAudioSource.Get(this);

        [Tooltip("Random pitch variation applied to each played clip.")]
        public Vector2 PitchRange = new(0.95f, 1.05f);

        [Tooltip("Random volume variation applied to each played clip.")]
        public Vector2 VolumeRange = new(0.85f, 0.95f);

        public void Play(AudioClip audioClip)
        {
            AudioSource.pitch = Random.Range(PitchRange.x, PitchRange.y);
            AudioSource.volume = Random.Range(VolumeRange.x, VolumeRange.y);
            AudioSource.PlayOneShot(audioClip);
        }

        private void Update()
        {
            if (!AudioSource.isPlaying)
            {
                OnOneShotComplete.Invoke();
            }
        }

        [Tooltip("Invoked when the audio source finishes playing.")]
        public UnityEvent OnOneShotComplete = new();
    }
}
