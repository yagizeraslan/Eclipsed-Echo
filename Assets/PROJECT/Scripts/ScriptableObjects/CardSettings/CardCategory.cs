using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    [CreateAssetMenu(fileName = "CardCategory", menuName = "Card Matching Game/Card Category")]
    public class CardCategory : ScriptableObject
    {
        public string categoryName;
        public List<string> cardSpriteAddresses;
    }
}
