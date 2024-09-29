using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    [CreateAssetMenu(fileName = "SoundSettings", menuName = "Card Matching Game/SoundSettings")]
    public class SoundSettings : ScriptableObject
    {
        public AudioClip cardFlipClip;
        public AudioClip cardMatchClip;
        public AudioClip cardNoMatchClip;
        public AudioClip levelCompletedClip;
    }
}