using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    public class CardManager : MonoBehaviour
    {
        private List<Card> flippedCards = new List<Card>();

        public async Task CardSelected(Card card)
        {
            if (card.IsMatched || card.IsFaceUp)
            {
                return;
            }

            card.FlipToFrontSide();
            flippedCards.Add(card);

            if (flippedCards.Count == 2)
            {
                await CompareFlippedCards(flippedCards[0], flippedCards[1]);
                flippedCards.Clear();
            }
        }

        private async Task CompareFlippedCards(Card firstCard, Card secondCard)
        {
            DisableCardsInteraction(firstCard, secondCard);

            await Task.Delay(500);

            GameManager.Instance.ScoreManager.IncrementTurns();

            if (firstCard.CardID == secondCard.CardID)
            {
                HandleMatchingCards(firstCard, secondCard);
            }
            else
            {
                HandleMismatchingCards(firstCard, secondCard);

                // Wait for 1 second before flipping back
                await Task.Delay(1000);

                firstCard.FlipToBackSide();
                secondCard.FlipToBackSide();
            }

            EnableCardsInteraction(firstCard, secondCard);

            if (CheckIfGameCompleted())
            {
                int bonusScore = GameManager.Instance.ScoreManager.CalculateBonusScore(
                    LevelManager.Instance.TotalPairs / 2,
                    GameManager.Instance.ScoreManager.Turns,
                    GameManager.Instance.ScoreManager.Matches * 100,
                    GameManager.Instance.ScoreManager.Turns * 10,
                    Mathf.FloorToInt(GameManager.Instance.TimerManager.Timer)
                );

                GameManager.Instance.ScoreManager.UpdateScore(bonusScore);
                DataPersistenceManager.Instance.SaveHighScore(GameManager.Instance.ScoreManager.Score);
                DataPersistenceManager.Instance.ClearSavedGame();
                await GameManager.Instance.GameOver();
            }
        }

        private void HandleMatchingCards(Card firstCard, Card secondCard)
        {
            GameManager.Instance.ScoreManager.IncrementMatches();
            GameManager.Instance.ScoreManager.UpdateScore(100);

            firstCard.Match();
            secondCard.Match();
            GameManager.Instance.SaveGame();
        }

        private void HandleMismatchingCards(Card firstCard, Card secondCard)
        {
            GameManager.Instance.ScoreManager.UpdateScore(-10);
            firstCard.Mismatch();
            secondCard.Mismatch();
        }

        private void DisableCardsInteraction(Card firstCard, Card secondCard)
        {
            firstCard.SetInteractable(false);
            secondCard.SetInteractable(false);
        }

        private void EnableCardsInteraction(Card firstCard, Card secondCard)
        {
            firstCard.SetInteractable(true);
            secondCard.SetInteractable(true);
        }

        private bool CheckIfGameCompleted()
        {
            int totalPairs = LevelManager.Instance.TotalPairs;
            return GameManager.Instance.ScoreManager.Matches >= totalPairs;
        }
    }
}
