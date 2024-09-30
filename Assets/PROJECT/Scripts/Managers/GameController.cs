using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YagizEraslan.EclipsedEcho
{
    public class GameController : MonoSingleton<GameController>
    {
        public UnityAction<int> OnScoreUpdated;
        public UnityAction<int> OnTurnsUpdated;
        public UnityAction<int> OnMatchesUpdated;

        private List<Card> flippedCards = new List<Card>(); // Cards waiting for comparison

        private int score;
        private int turns;
        private int matches;

        public int Score => score;
        public int Turns => turns;
        public int Matches => matches;

        public bool IsProcessing { get; private set; } // Remove global processing for flipping

        public void CardSelected(Card card)
        {
            // Ensure the card is not selected if it's already matched or face-up
            if (card.IsMatched || card.IsFaceUp)
            {
                return;
            }

            card.FlipToFrontSide(); // Flip the card immediately
            flippedCards.Add(card); // Add it to the list of flipped cards

            // Check if there are two flipped cards for comparison
            if (flippedCards.Count == 2)
            {
                StartCoroutine(CompareFlippedCards(flippedCards[0], flippedCards[1]));
                flippedCards.Clear(); // Clear the list for the next two cards
            }
        }

        private IEnumerator CompareFlippedCards(Card firstCard, Card secondCard)
        {
            // Lock these two cards to prevent interaction with them during comparison
            firstCard.SetInteractable(false);
            secondCard.SetInteractable(false);

            // Wait for a short delay before comparison
            yield return new WaitForSeconds(0.5f);

            // Increase the number of turns
            turns++;
            OnTurnsUpdated?.Invoke(turns);

            // Check if the two cards match
            if (firstCard.CardID == secondCard.CardID)
            {
                // Cards match
                matches++;
                score += 100;
                OnScoreUpdated?.Invoke(score);
                OnMatchesUpdated?.Invoke(matches);

                firstCard.Match(); // Mark both cards as matched
                secondCard.Match();
            }
            else
            {
                // Cards do not match
                score -= 10;
                OnScoreUpdated?.Invoke(score);

                firstCard.Mismatch();
                secondCard.Mismatch();

                // Wait for the mismatch animation before flipping back
                yield return new WaitForSeconds(1f);

                firstCard.FlipToBackSide();
                secondCard.FlipToBackSide();
            }

            // Unlock the cards after comparison
            firstCard.SetInteractable(true);
            secondCard.SetInteractable(true);

            // Check if all pairs are matched
            if (CheckIfGameCompleted())
            {
                GameManager.Instance.GameOver();
            }
        }

        private bool CheckIfGameCompleted()
        {
            int totalPairs = LevelManager.Instance.TotalPairs;
            return matches >= totalPairs;
        }
    }

}
