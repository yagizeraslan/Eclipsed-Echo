using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.XR;
using System.Text.RegularExpressions;

namespace YagizEraslan.EclipsedEcho
{
    public class UIManager : MonoBehaviour
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
        public TextMeshProUGUI LevelCompletedTimeText => levelCompletedTimeText;
        [SerializeField] private TextMeshProUGUI levelCompletedBonusText;
        [SerializeField] private TextMeshProUGUI levelCompletedScoreText;

        [Header("Resume Game? Buttons")]
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        [Header("Level Completed Buttons")]
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button mainMenuButton;

        public static UIManager Instance;

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
            highScoreText.text = $"High Score: {DataPersistenceManager.Instance.LoadHighScore().ToString()}";

            GameManager.Instance.ScoreManager.OnScoreUpdated += UpdateScore;
            GameManager.Instance.ScoreManager.OnBonusUpdated += UpdateBonus;
            GameManager.Instance.ScoreManager.OnTurnsUpdated += UpdateTurns;
            GameManager.Instance.ScoreManager.OnMatchesUpdated += UpdateMatches;
            GameManager.Instance.TimerManager.OnTimerUpdated += UpdateTimer;

            yesButton.onClick.AddListener(GameController.Instance.LoadGameLevel);
            noButton.onClick.AddListener(GameController.Instance.DeclineLoadLevel);

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

        public void ResetAllUIValues()
        {
            scoreText.text = $": 0.0";
            timerText.text = $": 0.0";
            turnsText.text = $": 0.0";
            matchesText.text = $": 0.0";
        }

        public void UpdateLevelCompletedUI()
        {
            int finalScore = ScoreManager.Instance.Score;
            int turns = ScoreManager.Instance.Turns;
            int bonus = ScoreManager.Instance.Bonus;

            levelCompletedTurnsText.text = $"Turns: {turns}";
            levelCompletedTimeText.text = $"Completed in {Mathf.Round(TimerManager.Instance.Timer)} seconds";
            levelCompletedBonusText.text = $"Bonus: {bonus}";
            levelCompletedScoreText.text = $"Score: {finalScore}";
        }

        private void RestartLevel()
        {
            TimerManager.Instance.ResetTimer();
            int selectedGridSize = DataPersistenceManager.Instance.SelectedGridSize;
            int selectedCategoryKey = DataPersistenceManager.Instance.SelectedCategoryKey;
            LevelManager.Instance.GenerateLevel(selectedGridSize, selectedCategoryKey);
        }

        private void MainMenu()
        {
            TimerManager.Instance.ResetTimer();
            highScoreText.text = $"High Score: {DataPersistenceManager.Instance.LoadHighScore().ToString()}";
            GameManager.Instance.ShowMainMenuPanel();
        }

        private void OnDestroy()
        {
            GameManager.Instance.ScoreManager.OnScoreUpdated -= UpdateScore;
            GameManager.Instance.ScoreManager.OnBonusUpdated -= UpdateBonus;
            GameManager.Instance.ScoreManager.OnTurnsUpdated -= UpdateTurns;
            GameManager.Instance.ScoreManager.OnMatchesUpdated -= UpdateMatches;
            GameManager.Instance.TimerManager.OnTimerUpdated -= UpdateTimer;

            yesButton.onClick.RemoveListener(GameController.Instance.LoadGameLevel);
            noButton.onClick.RemoveListener(GameController.Instance.DeclineLoadLevel);

            restartLevelButton.onClick.RemoveListener(RestartLevel);
            mainMenuButton.onClick.RemoveListener(MainMenu);
        }
    }
}
