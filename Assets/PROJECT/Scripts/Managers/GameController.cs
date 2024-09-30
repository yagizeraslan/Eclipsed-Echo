using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

namespace YagizEraslan.EclipsedEcho
{
    public class GameController : MonoSingleton<GameController>
    {
        public UnityAction<int> OnScoreUpdated;
        public UnityAction<int> OnBonusUpdated;
        public UnityAction<int> OnTurnsUpdated;
        public UnityAction<int> OnMatchesUpdated;
        public UnityAction<float> OnTimerUpdated;

        private List<Card> flippedCards = new List<Card>();

        private int score;
        private int bonus;
        private int turns;
        private int matches;
        private float timer;
        private bool isTiming;
        private float lastDisplayedTime = 0f;

        public int Score => score;
        public int Bonus => bonus;
        public int Turns => turns;
        public int Matches => matches;
        public float Timer => timer;

        public bool IsProcessing { get; private set; }

        public void InitializeStartingValues()
        {
            score = 0;
            bonus = 0;
            turns = 0;
            matches = 0;
            timer = 0f;
            isTiming = false;

            OnScoreUpdated?.Invoke(score);
            OnBonusUpdated?.Invoke(bonus);
            OnTurnsUpdated?.Invoke(turns);
            OnMatchesUpdated?.Invoke(matches);
            OnTimerUpdated?.Invoke(timer);
        }

        private void Update()
        {
            if (isTiming)
            {
                UpdateTimer(Time.deltaTime);
            }
        }

        private void UpdateTimer(float deltaTime)
        {
            timer += deltaTime;

            if (Mathf.FloorToInt(timer) != Mathf.FloorToInt(lastDisplayedTime))
            {
                lastDisplayedTime = timer;
                OnTimerUpdated?.Invoke(timer);
            }
        }

        public void StartTimer()
        {
            isTiming = true;
            timer = 0f;
            OnTimerUpdated?.Invoke(timer);
        }

        public void StopTimer()
        {
            isTiming = false;
        }

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
            }

            firstCard.SetInteractable(true);
            secondCard.SetInteractable(true);

            if (CheckIfGameCompleted())
            {
                score += CalculateBonusScore(LevelManager.Instance.TotalPairs / 2, turns, matches * 100, turns * 10, Mathf.FloorToInt(timer));
                DataPersistenceManager.Instance.SaveHighScore(score);
                GameManager.Instance.GameOver();
            }
        }

        private void DisableCardsInteraction(Card firstCard, Card secondCard)
        {
            firstCard.SetInteractable(false);
            secondCard.SetInteractable(false);
        }

        private int CalculateBonusScore(int totalImages, int turns, int baseScore, int turnPenalty, int timePenalty)
        {
            int bonusMultiplier = 2;

            if (totalImages == turns)
            {
                bonus = (baseScore - turnPenalty - timePenalty) * bonusMultiplier;
                OnBonusUpdated?.Invoke(bonus);
            }
            else if (totalImages + 1 == turns)
            {
                bonus = 200;
                OnBonusUpdated?.Invoke(bonus);
            }
            else if (totalImages + 2 == turns)
            {
                bonus = 100;
                OnBonusUpdated?.Invoke(bonus);
            }
            else if (totalImages + 3 == turns)
            {
                bonus = 50;
                OnBonusUpdated?.Invoke(bonus);
            }
            else
            {
                bonus = 0;
                OnBonusUpdated?.Invoke(bonus);
            }

            return bonus;
        }

        private bool CheckIfGameCompleted()
        {
            int totalPairs = LevelManager.Instance.TotalPairs;
            return matches >= totalPairs;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnGameOver -= StopTimer;
            OnScoreUpdated = null;
            OnBonusUpdated = null;
            OnTurnsUpdated = null;
            OnMatchesUpdated = null;
            OnTimerUpdated = null;
        }
    }
}