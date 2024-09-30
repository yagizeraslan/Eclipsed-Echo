using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YagizEraslan.EclipsedEcho
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public UnityAction OnGameStart;
        public UnityAction OnGameOver;

        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject resumePanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject levelCompletedPanel;

        private void Start()
        {
            ShowMainMenu();
            CheckForSavedGame();
        }

        private void ShowMainMenu()
        {
            mainMenuPanel.SetActive(true);
            gameplayPanel.SetActive(false);
            levelCompletedPanel.SetActive(false);
            resumePanel.SetActive(false);
        }

        private void CheckForSavedGame()
        {
            if (DataPersistenceManager.Instance.HasSavedGame())
            {
                resumePanel.SetActive(true);
            }
        }

        public void StartGame()
        {
            mainMenuPanel.SetActive(false);
            gameplayPanel.SetActive(true);
            OnGameStart?.Invoke();
        }

        public void GameOver()
        {
            gameplayPanel.SetActive(false);
            levelCompletedPanel.SetActive(true);
            OnGameOver?.Invoke();
        }

        public void ResumeGame()
        {
            resumePanel.SetActive(false);
            gameplayPanel.SetActive(true);
            OnGameStart?.Invoke();
        }

        public void ReturnToMainMenu()
        {
            levelCompletedPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }
}