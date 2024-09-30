using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace YagizEraslan.EclipsedEcho
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [Header("Ongoing Gameplay Texts")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI turnsText;
        [SerializeField] private TextMeshProUGUI matchesText;
        [SerializeField] private TextMeshProUGUI highScoreText;

        [Header("Completed Gameplay Texts")]
        [SerializeField] private TextMeshProUGUI levelCompletedTurnsText;
        [SerializeField] private TextMeshProUGUI levelCompletedTimeText;
        [SerializeField] private TextMeshProUGUI levelCompletedBonusText;
        [SerializeField] private TextMeshProUGUI levelCompletedScoreText;

        [Header("Buttons")]
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            if (DataPersistenceManager.Instance.LoadHighScore().ToString() != null) 
            {
                highScoreText.text = $"High Score: {DataPersistenceManager.Instance.LoadHighScore().ToString()}";
            }

            GameManager.Instance.OnGameStart += StartTimer;
            GameManager.Instance.OnGameOver += StopTimer;

            GameController.Instance.OnScoreUpdated += UpdateScore;
            GameController.Instance.OnBonusUpdated += UpdateBonus;
            GameController.Instance.OnTurnsUpdated += UpdateTurns;
            GameController.Instance.OnMatchesUpdated += UpdateMatches;
            GameController.Instance.OnTimerUpdated += UpdateTimer;

            restartLevelButton.onClick.AddListener(RestartLevel);
            mainMenuButton.onClick.AddListener(MainMenu);
        }

        private void UpdateScore(int score)
        {
            scoreText.text = $": {score}";
        }

        private void UpdateBonus(int bonus)
        {
            levelCompletedBonusText.text = $": {bonus}";
        }

        private void UpdateTurns(int turns)
        {
            turnsText.text = $": {turns}";
        }

        private void UpdateMatches(int matches)
        {
            matchesText.text = $": {matches}";
        }

        private void UpdateTimer(float timer)
        {
            timerText.text = $": {timer.ToString("F1")}";
        }

        private void StartTimer()
        {
            GameController.Instance.StartTimer();
        }

        private void StopTimer()
        {
            GameController.Instance.StopTimer();
            UpdateLevelCompletedUI();
        }

        private void UpdateLevelCompletedUI()
        {
            int finalScore = GameController.Instance.Score;
            int turns = GameController.Instance.Turns;
            int bonus = GameController.Instance.Bonus;

            levelCompletedTurnsText.text = $"Turns: {turns}";
            levelCompletedTimeText.text = $"Completed in {Mathf.Round(GameController.Instance.Timer)} seconds";
            levelCompletedBonusText.text = $"Bonus: {bonus}";
            levelCompletedScoreText.text = $"Score: {finalScore}";
        }

        private void RestartLevel()
        {
            GameController.Instance.InitializeStartingValues();
            GameManager.Instance.ShowGameplayPanel();
            MainMenuManager.Instance.GenerateSelectedLevel();
        }

        private void MainMenu()
        {
            highScoreText.text = $"High Score: {DataPersistenceManager.Instance.LoadHighScore().ToString()}";
            GameManager.Instance.ShowMainMenuPanel();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnGameStart -= StartTimer;
            GameManager.Instance.OnGameOver -= StopTimer;

            GameController.Instance.OnScoreUpdated -= UpdateScore;
            GameController.Instance.OnBonusUpdated -= UpdateBonus;
            GameController.Instance.OnTurnsUpdated -= UpdateTurns;
            GameController.Instance.OnMatchesUpdated -= UpdateMatches;
            GameController.Instance.OnTimerUpdated -= UpdateTimer;

            restartLevelButton.onClick.RemoveListener(RestartLevel);
            mainMenuButton.onClick.RemoveListener(GameManager.Instance.ShowMainMenuPanel);
        }
    }
}