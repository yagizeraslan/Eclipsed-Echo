using UnityEngine;
using UnityEngine.Events;

namespace YagizEraslan.EclipsedEcho
{
    public class ScoreManager : MonoSingleton<ScoreManager>
    {
        public UnityAction<int> OnScoreUpdated;
        public UnityAction<int> OnBonusUpdated;
        public UnityAction<int> OnTurnsUpdated;
        public UnityAction<int> OnMatchesUpdated;

        private int score;
        private int bonus;
        private int turns;
        private int matches;

        public int Score => score;
        public int Bonus => bonus;
        public int Turns => turns;
        public int Matches => matches;

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
            score = Mathf.Max(0, score + value); // Ensuring score doesn't go negative.
            OnScoreUpdated?.Invoke(score);
        }

        public void IncrementMatches()
        {
            matches++;
            OnMatchesUpdated?.Invoke(matches);
        }

        public int CalculateBonusScore(int totalPairs, int turns, int baseScore, int turnPenalty, int timePenalty)
        {
            int bonusMultiplier = 2;

            if (totalPairs == turns)
            {
                bonus = (baseScore - turnPenalty - timePenalty) * bonusMultiplier;
            }
            else if (totalPairs + 1 == turns)
            {
                bonus = 200;
            }
            else if (totalPairs + 2 == turns)
            {
                bonus = 100;
            }
            else if (totalPairs + 3 == turns)
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
