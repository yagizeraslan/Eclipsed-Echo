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
            Application.targetFrameRate = 60;

            ShowMainMenuPanel();
            //CheckForSavedGame();
        }

        public void ShowMainMenuPanel()
        {
            mainMenuPanel.SetActive(true);
            resumePanel.SetActive(false);
            gameplayPanel.SetActive(false);
            levelCompletedPanel.SetActive(false);
        }

        //private void CheckForSavedGame()
        //{
        //    if (DataPersistenceManager.Instance.HasSavedGame())
        //    {
        //        resumePanel.SetActive(true);
        //    }
        //}

        public void ShowGameplayPanel()
        {
            mainMenuPanel.SetActive(false);
            resumePanel.SetActive(false);
            gameplayPanel.SetActive(true);
            levelCompletedPanel.SetActive(false);
            OnGameStart?.Invoke();
        }

        public void GameOver()
        {
            StartCoroutine(HandleLevelComplete());
        }

        private IEnumerator HandleLevelComplete()
        {
            yield return new WaitForSeconds(0.5f);
            SoundManager.Instance.PlayLevelCompletedSound();
            yield return new WaitForSeconds(0.5f);
            LevelManager.Instance.ClearGrid();
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

        

        //public void RestartGame()
        //{
        //    DataPersistenceManager.Instance.SaveSelectedGridType();
        //}
    }
}