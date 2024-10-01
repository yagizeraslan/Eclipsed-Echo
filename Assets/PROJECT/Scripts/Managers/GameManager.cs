using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace YagizEraslan.EclipsedEcho
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public ScoreManager ScoreManager { get; private set; }
        public TimerManager TimerManager { get; private set; }
        public CardManager CardManager { get; private set; }

        public UnityAction OnGameStart;

        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject resumePanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject levelCompletedPanel;

        protected override void Awake()
        {
            base.Awake(); // Ensure MonoSingleton Awake method is called
            ScoreManager = gameObject.AddComponent<ScoreManager>();
            TimerManager = gameObject.AddComponent<TimerManager>();
            CardManager = gameObject.AddComponent<CardManager>();
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            ShowMainMenuPanel();
            CheckForSavedGame();

            OnGameStart?.Invoke(); // Invoke the event when the game starts
        }

        public void ShowMainMenuPanel()
        {
            mainMenuPanel.SetActive(true);
            resumePanel.SetActive(false);
            gameplayPanel.SetActive(false);
            levelCompletedPanel.SetActive(false);
        }

        private void CheckForSavedGame()
        {
            if (DataPersistenceManager.Instance.HasSavedGame())
            {
                resumePanel.SetActive(true);
            }
        }

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
            UIManager.Instance.UpdateLevelCompletedUI();
            yield return new WaitForSeconds(0.5f);
            SoundManager.Instance.PlayLevelCompletedSound();
            yield return new WaitForSeconds(0.5f);
            LevelManager.Instance.ClearGrid();
            gameplayPanel.SetActive(false);
            levelCompletedPanel.SetActive(true);
        }

        public void ResumeGame()
        {
            GameController.Instance.LoadGameFromState(DataPersistenceManager.Instance.LoadGameState());
            ShowGameplayPanel();
        }

        public void SaveGame()
        {
            GameController.Instance.SaveGame();
        }
    }
}
