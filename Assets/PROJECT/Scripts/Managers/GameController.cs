using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    public class GameController : MonoSingleton<GameController>
    {
        public ScoreManager ScoreManager => GameManager.Instance.ScoreManager;
        public TimerManager TimerManager => GameManager.Instance.TimerManager;
        public CardManager CardManager => GameManager.Instance.CardManager;

        private void Start()
        {
            InitializeStartingValues();
        }

        public void InitializeStartingValues()
        {
            ScoreManager.InitializeStartingValues();
            TimerManager.StartTimer();
        }

        private void Update()
        {
            TimerManager.UpdateTimer(Time.deltaTime);
        }

        public void SaveGame()
        {
            List<CardData> cardData = LevelManager.Instance.GetCardsData();
            DataPersistenceManager.Instance.SaveGameState(cardData, ScoreManager.Score, TimerManager.Timer, ScoreManager.Turns, ScoreManager.Matches);
        }

        public void LoadGameFromState(GameState gameState)
        {
            LevelManager.Instance.LoadCardsFromState(gameState.cardData);
            ScoreManager.InitializeStartingValues();
            ScoreManager.UpdateScore(gameState.score);
            TimerManager.StartTimer();
        }

        public void CardSelected(Card card)
        {
            CardManager.CardSelected(card);
        }
    }
}
