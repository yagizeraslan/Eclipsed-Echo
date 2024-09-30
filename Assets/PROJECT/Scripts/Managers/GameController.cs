using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YagizEraslan.EclipsedEcho
{
    public class GameController : MonoSingleton<GameController>
    {
        public bool IsProcessing { get; private set; }

        public UnityAction<int> OnScoreUpdated;
        public UnityAction<int> OnTurnsUpdated;
        public UnityAction<int> OnMatchesUpdated;

        private Card firstSelectedCard;
        private Card secondSelectedCard;

        private int score;
        private int turns;
        private int matches;

        // Public properties to access private fields
        public int Score => score;
        public int Turns => turns;
        public int Matches => matches;

        public void CardSelected(Card card)
        {
            if (firstSelectedCard == null)
            {
                firstSelectedCard = card;
            }
            else if (secondSelectedCard == null && card != firstSelectedCard)
            {
                secondSelectedCard = card;
                IsProcessing = true;

                // Start a coroutine to wait before comparing cards
                StartCoroutine(WaitForSecondFlipAndCompare());
            }
        }

        private IEnumerator WaitForSecondFlipAndCompare()
        {
            // Wait for the duration of the flip animation
            yield return new WaitForSeconds(0.5f); // Adjust duration to match your flip animation

            CompareCards();
        }

        private void CompareCards()
        {
            IsProcessing = true;
            turns++;
            OnTurnsUpdated?.Invoke(turns);

            if (firstSelectedCard.CardID == secondSelectedCard.CardID)
            {
                matches++;
                score += 100;
                OnScoreUpdated?.Invoke(score);
                OnMatchesUpdated?.Invoke(matches);

                firstSelectedCard.Match();
                secondSelectedCard.Match();

                Invoke(nameof(DisableMatchedCards), 1f);
            }
            else
            {
                score -= 10;
                OnScoreUpdated?.Invoke(score);

                firstSelectedCard.Mismatch();
                secondSelectedCard.Mismatch();

                Invoke(nameof(ResetCards), 1f);
            }
        }

        private void DisableMatchedCards()
        {
            // Interaction is already disabled in Card.Match()
            // Optionally, you can disable the cards here if necessary

            ResetSelection();
            IsProcessing = false;

            if (CheckIfGameCompleted())
            {
                GameManager.Instance.GameOver();
            }
        }

        private void ResetCards()
        {
            firstSelectedCard.FlipToBackSide();
            secondSelectedCard.FlipToBackSide();
            ResetSelection();
            IsProcessing = false;
        }

        private void ResetSelection()
        {
            firstSelectedCard = null;
            secondSelectedCard = null;
        }

        private bool CheckIfGameCompleted()
        {
            int totalPairs = LevelManager.Instance.TotalPairs;
            return matches >= totalPairs;
        }
    }
}
