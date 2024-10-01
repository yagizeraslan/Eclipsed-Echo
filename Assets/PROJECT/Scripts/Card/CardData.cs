namespace YagizEraslan.EclipsedEcho
{
    [System.Serializable]
    public class CardData
    {
        public int CardID { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
    }
}