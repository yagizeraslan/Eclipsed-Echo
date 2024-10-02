using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    public class CardManager : MonoBehaviour
    {
        private List<Card> flippedCards = new List<Card>();

        public void CardSelected(Card card)
        {
            if (card.IsMatched || card.IsFaceUp)
            {
                return;
            }

            card.FlipToFrontSide();
            flippedCards.Add(card);

            if (flippedCards.Count == 2)
            {
                StartCoroutine(CompareFlippedCards(flippedCards[0], flippedCards[1]));
                flippedCards.Clear();
            }
        }

        private IEnumerator CompareFlippedCards(Card firstCard, Card secondCard)
        {
            DisableCardsInteraction(firstCard, secondCard);

            yield return new WaitForSeconds(0.5f);

            GameManager.Instance.ScoreManager.IncrementTurns();

            if (firstCard.CardID == secondCard.CardID)
            {
                HandleMatchingCards(firstCard, secondCard);
            }
            else
            {
                HandleMismatchingCards(firstCard, secondCard);
                yield return new WaitForSeconds(1f);
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
                    Mathf.FloorToInt(GameManager.Instance.TimerManager.Timer)
                );

                GameManager.Instance.ScoreManager.UpdateScore(bonusScore);
                DataPersistenceManager.Instance.SaveHighScore(GameManager.Instance.ScoreManager.Score);
                DataPersistenceManager.Instance.ClearSavedGame();
                GameManager.Instance.GameOver();
            }
        }

        private void HandleMatchingCards(Card firstCard, Card secondCard)
        {
            GameManager.Instance.ScoreManager.IncrementMatches();
            GameManager.Instance.ScoreManager.UpdateScore(ScoreManager.Instance.MatchingScore);

            firstCard.Match();
            secondCard.Match();
            GameController.Instance.SaveGameLevel();
        }

        private void HandleMismatchingCards(Card firstCard, Card secondCard)
        {
            GameManager.Instance.ScoreManager.UpdateScore(ScoreManager.Instance.MismatchingScore);
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
