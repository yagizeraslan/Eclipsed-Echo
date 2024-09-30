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
            GameManager.Instance.OnGameStart += StartTimer;
            GameManager.Instance.OnGameOver += StopTimer;

            GameController.Instance.OnScoreUpdated += UpdateScore;
            GameController.Instance.OnBonusUpdated += UpdateBonus;
            GameController.Instance.OnTurnsUpdated += UpdateTurns;
            GameController.Instance.OnMatchesUpdated += UpdateMatches;
            GameController.Instance.OnTimerUpdated += UpdateTimer;

            // Add listeners to buttons
            restartLevelButton.onClick.AddListener(RestartLevel);
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
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
            levelCompletedScoreText.text = $"Score: {finalScore + bonus}";
        }

        private void RestartLevel()
        {
            GameController.Instance.InitializeStartingValues();
            LevelManager.Instance.RestartCurrentLevel();
        }

        private void ReturnToMainMenu()
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}