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

        private List<Card> flippedCards = new List<Card>();
        private bool isComparing = false;

        private int score;
        private int turns;
        private int matches;

        public int Score => score;
        public int Turns => turns;
        public int Matches => matches;

        public bool IsProcessing { get; private set; }

        public void CardSelected(Card card)
        {
            if (card.IsMatched || card.IsFaceUp || isComparing)
                return;

            card.FlipToFrontSide();
            flippedCards.Add(card);

            if (flippedCards.Count >= 2)
            {
                StartCoroutine(CompareFlippedCards());
            }
        }

        public void SetProcessing(bool processing)
        {
            IsProcessing = processing;
        }

        private IEnumerator CompareFlippedCards()
        {
            isComparing = true;

            // Wait for flip animations to complete
            yield return new WaitForSeconds(0.5f);

            // Compare cards in pairs
            while (flippedCards.Count >= 2)
            {
                Card firstCard = flippedCards[0];
                Card secondCard = flippedCards[1];

                turns++;
                OnTurnsUpdated?.Invoke(turns);

                if (firstCard.CardID == secondCard.CardID)
                {
                    matches++;
                    score += 100;
                    OnScoreUpdated?.Invoke(score);
                    OnMatchesUpdated?.Invoke(matches);

                    firstCard.Match();
                    secondCard.Match();

                    flippedCards.Remove(firstCard);
                    flippedCards.Remove(secondCard);
                }
                else
                {
                    score -= 10;
                    OnScoreUpdated?.Invoke(score);

                    firstCard.Mismatch();
                    secondCard.Mismatch();

                    yield return new WaitForSeconds(1f);

                    firstCard.FlipToBackSide();
                    secondCard.FlipToBackSide();

                    flippedCards.Remove(firstCard);
                    flippedCards.Remove(secondCard);
                }

                yield return new WaitForSeconds(0.5f);
            }

            isComparing = false;

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
