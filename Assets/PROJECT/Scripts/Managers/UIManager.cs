using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

        private float timer;
        private bool isTiming;

        private void Start()
        {
            GameManager.Instance.OnGameStart += StartTimer;
            GameManager.Instance.OnGameOver += StopTimer;

            GameController.Instance.OnScoreUpdated += UpdateScore;
            GameController.Instance.OnTurnsUpdated += UpdateTurns;
            GameController.Instance.OnMatchesUpdated += UpdateMatches;
        }

        private void Update()
        {
            if (isTiming)
            {
                timer += Time.deltaTime;
                //timerText.text = $"Timer: {timer:F2}";

                // Calculate minutes and seconds
                int minutes = Mathf.FloorToInt(timer / 60);
                int seconds = Mathf.FloorToInt(timer % 60);

                // Update the timer text in the format "00:00"
                timerText.text = string.Format(": {0:00}:{1:00}", minutes, seconds);
            }
        }

        private void StartTimer()
        {
            isTiming = true;
            timer = 0f;
        }

        private void StopTimer()
        {
            isTiming = false;
            UpdateLevelCompletedUI();
        }

        private void UpdateScore(int score)
        {
            scoreText.text = $"Score: {score}";
        }

        private void UpdateTurns(int turns)
        {
            turnsText.text = $"Turns: {turns}";
        }

        private void UpdateMatches(int matches)
        {
            matchesText.text = $"Matches: {matches}";
        }

        private void UpdateLevelCompletedUI()
        {
            int finalScore = GameController.Instance.Score;
            int turns = GameController.Instance.Turns;
            int bonus = CalculateBonus();

            levelCompletedTurnsText.text = $"Turns: {turns}";
            levelCompletedTimeText.text = $"Completed in {timer:F2} seconds";
            levelCompletedBonusText.text = $"Bonus: {bonus}";
            levelCompletedScoreText.text = $"Score: {finalScore + bonus}";
        }

        private int CalculateBonus()
        {
            // Implement bonus calculation based on time or other factors
            return 0;
        }
    }
}