using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        [Header("Level Completed Buttons")]
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        [Header("Level Completed Buttons")]
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            highScoreText.text = $"High Score: {DataPersistenceManager.Instance.LoadHighScore().ToString()}";

            GameManager.Instance.ScoreManager.OnScoreUpdated += UpdateScore;
            GameManager.Instance.ScoreManager.OnBonusUpdated += UpdateBonus;
            GameManager.Instance.ScoreManager.OnTurnsUpdated += UpdateTurns;
            GameManager.Instance.ScoreManager.OnMatchesUpdated += UpdateMatches;
            GameManager.Instance.TimerManager.OnTimerUpdated += UpdateTimer;

            yesButton.onClick.AddListener(DataPersistenceManager.Instance.LoadLevel);
            noButton.onClick.AddListener(DataPersistenceManager.Instance.DeclineLoadLevel);

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
            int selectedGridSize = DataPersistenceManager.Instance.SelectedGridSize;
            int selectedCategoryKey = DataPersistenceManager.Instance.SelectedCategoryKey;
            MainMenuManager.Instance.GenerateCustomLevel(selectedGridSize, selectedCategoryKey);
        }

        private void MainMenu()
        {
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

            yesButton.onClick.RemoveListener(DataPersistenceManager.Instance.LoadLevel);
            noButton.onClick.RemoveListener(DataPersistenceManager.Instance.DeclineLoadLevel);

            restartLevelButton.onClick.RemoveListener(RestartLevel);
            mainMenuButton.onClick.RemoveListener(MainMenu);
        }
    }
}
