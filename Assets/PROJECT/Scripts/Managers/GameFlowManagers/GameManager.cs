using System.Threading.Tasks;
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
            base.Awake();
            ScoreManager = gameObject.AddComponent<ScoreManager>();
            TimerManager = gameObject.AddComponent<TimerManager>();
            CardManager = gameObject.AddComponent<CardManager>();
        }

        private void Start()
        {
            Application.targetFrameRate = 60;

            ShowMainMenuPanel();
            CheckForSavedGame();

            OnGameStart?.Invoke();
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

        public async Task GameOver()
        {
            await HandleLevelCompleteAsync();
        }

        private async Task HandleLevelCompleteAsync()
        {
            UIManager.Instance.UpdateLevelCompletedUI();
            await Task.Delay(500);
            SoundManager.Instance.PlayLevelCompletedSound();
            await Task.Delay(500);

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
