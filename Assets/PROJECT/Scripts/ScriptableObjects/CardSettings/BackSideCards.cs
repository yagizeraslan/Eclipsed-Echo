using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    [CreateAssetMenu(fileName = "BackSideCards", menuName = "Card Matching Game/Back Side Cards")]
    public class BackSideCards : ScriptableObject
    {
        public List<string> backSideSpriteAddresses;
    }
}
