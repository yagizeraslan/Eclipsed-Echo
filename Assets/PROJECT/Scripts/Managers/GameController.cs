using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    public class GameController : MonoBehaviour
    {
        public ScoreManager ScoreManager => GameManager.Instance.ScoreManager;
        public TimerManager TimerManager => GameManager.Instance.TimerManager;
        public CardManager CardManager => GameManager.Instance.CardManager;

        public static GameController Instance;

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

        private void Start()
        {
            ScoreManager.InitializeStartingValues();
        }

        private void Update()
        {
            TimerManager.UpdateTimer(Time.deltaTime);
        }

        public void SaveGameLevel()
        {
            DataPersistenceManager.Instance.SaveGameState(LevelManager.Instance.GetCardsData(), ScoreManager.Score, TimerManager.Timer, ScoreManager.Turns, ScoreManager.Matches);
        }

        public void LoadGameLevel()
        {
            DataPersistenceManager.Instance.ClearSavedGame();
            LevelManager.Instance.GenerateLevel(DataPersistenceManager.Instance.SelectedGridSize, DataPersistenceManager.Instance.SelectedCategoryKey);
        }

        public void DeclineLoadLevel()
        {
            DataPersistenceManager.Instance.ClearSavedGame();
            GameManager.Instance.ShowMainMenuPanel();
        }

        public void CardSelected(Card card)
        {
            CardManager.CardSelected(card);
        }
    }
}
