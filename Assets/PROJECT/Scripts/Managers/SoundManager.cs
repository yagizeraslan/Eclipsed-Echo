using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField] private SoundSettings soundSettings;

        private AudioSource audioSource;

        protected override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayFlipSound()
        {
            audioSource.PlayOneShot(soundSettings.cardFlipClip);
        }

        public void PlayMatchSound()
        {
            audioSource.PlayOneShot(soundSettings.cardMatchClip);
        }

        public void PlayMismatchSound()
        {
            audioSource.PlayOneShot(soundSettings.cardNoMatchClip);
        }

        public void PlayLevelCompletedSound()
        {
            audioSource.PlayOneShot(soundSettings.levelCompletedClip);
        }
    }
}
