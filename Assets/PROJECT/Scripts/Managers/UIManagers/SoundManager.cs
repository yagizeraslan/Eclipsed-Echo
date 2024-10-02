using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private SoundSettings soundSettings;

        private AudioSource audioSource;

        public static SoundManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

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
