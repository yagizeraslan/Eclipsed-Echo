using UnityEngine;
using UnityEngine.Events;

namespace YagizEraslan.EclipsedEcho
{
    public class ScoreManager : MonoBehaviour
    {
        public UnityAction<int> OnScoreUpdated;
        public UnityAction<int> OnBonusUpdated;
        public UnityAction<int> OnTurnsUpdated;
        public UnityAction<int> OnMatchesUpdated;

        private int matchingScore = 100;
        private int mismatchingScore = -10;

        private int score;
        private int bonus;
        private int turns;
        private int matches;

        public int MatchingScore => matchingScore;
        public int MismatchingScore => mismatchingScore;
        public int Score => score;
        public int Bonus => bonus;
        public int Turns => turns;
        public int Matches => matches;

        public static ScoreManager Instance;

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
        }

        public void InitializeStartingValues()
        {
            score = 0;
            bonus = 0;
            turns = 0;
            matches = 0;

            OnScoreUpdated?.Invoke(score);
            OnBonusUpdated?.Invoke(bonus);
            OnTurnsUpdated?.Invoke(turns);
            OnMatchesUpdated?.Invoke(matches);
        }

        public void IncrementTurns()
        {
            turns++;
            OnTurnsUpdated?.Invoke(turns);
        }

        public void UpdateScore(int value)
        {
            score = Mathf.Max(0, score + value); //Ensuring score doesn't go negative.
            OnScoreUpdated?.Invoke(score);
        }

        public void IncrementMatches()
        {
            matches++;
            OnMatchesUpdated?.Invoke(matches);
        }

        public int CalculateBonusScore(int totalPairs, int turns, int baseScore, int timePenalty)
        {

            int adjustedBaseScore = baseScore - timePenalty;

            if (adjustedBaseScore < 0)
            {
                adjustedBaseScore = 0;
            }

            if (turns == totalPairs)
            {
                int bonusMultiplier = 4;
                bonus = adjustedBaseScore * bonusMultiplier;
            }
            else if (turns <= totalPairs + 2)
            {
                int bonusMultiplier = 2;
                bonus = adjustedBaseScore * bonusMultiplier;
            }
            else if (turns <= totalPairs + 4)
            {
                bonus = 100;
            }
            else if (turns <= totalPairs + 6)
            {
                bonus = 50;
            }
            else
            {
                bonus = 0;
            }

            OnBonusUpdated?.Invoke(bonus);
            return bonus;
        }
    }
}
