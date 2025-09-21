using AdvancedTurrets.Behaviours.Lifecycles;
using AdvancedTurrets.Libraries;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Spawners
{
    /// <summary>
    /// Custom spawner for an <see cref="AudioSourceRelay"/> that configures its audio source before playing.
    /// </summary>
    public class AudioPlayerSpawner : BaseSpawner<AudioSourceRelay>
    {
        [Tooltip("The collection of audio clips from which the AudioPlayerSpawner will select.")]
        public AudioClip[] AudioClips;

        [Tooltip("True: The selected audio clip will be chosen randomly. False: The audio clips will be played in order.")]
        public bool Randomize = true;

        private int _index = 0;

        /// <summary>
        /// Selects a random audio clip from the available collection.
        /// </summary>
        private AudioClip GetRandomAudioClip()
        {
            return (AudioClips?.Length > 0) ? AudioClips.RandomOrDefault() : null;
        }

        /// <summary>
        /// Selects the next audio clip based on the settings.
        /// </summary>
        private AudioClip GetNextAudioClip()
        {
            if (Randomize)
            {
                return GetRandomAudioClip();
            }
            else if (AudioClips?.Length > 0)
            {
                var clip = AudioClips[_index];
                _index = (_index + 1) % AudioClips.Length; // Ensures looping through clips without out-of-bounds access.
                return clip;
            }

            return null;
        }

        /// <summary>
        /// Called after an <see cref="AudioSourceRelay"/> is spawned.
        /// Assigns and plays an audio clip.
        /// </summary>
        protected override void OnSpawned(AudioSourceRelay audioPlayer)
        {
            base.OnSpawned(audioPlayer);
            var audioClip = GetNextAudioClip();

            if (audioClip != null)
            {
                audioPlayer.Play(audioClip);
            }
            else
            {
                Debug.LogWarning($"No audio clip selected for {name}");
            }
        }
    }
}
