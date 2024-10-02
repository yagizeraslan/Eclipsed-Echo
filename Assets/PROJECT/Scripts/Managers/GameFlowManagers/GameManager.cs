using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace YagizEraslan.EclipsedEcho
{
    public class GameManager : MonoBehaviour
    {
        private const int SHORT_DELAY = 500;
        public ScoreManager ScoreManager { get; private set; }
        public TimerManager TimerManager { get; private set; }
        public CardManager CardManager { get; private set; }

        public UnityAction OnGameStart;

        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject resumePanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject levelCompletedPanel;


        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(gameObject);
            }

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
            TimerManager.Instance.StopTimer();
            UIManager.Instance.UpdateLevelCompletedUI();
            await Task.Delay(SHORT_DELAY);
            SoundManager.Instance.PlayLevelCompletedSound();
            await Task.Delay(SHORT_DELAY);

            LevelManager.Instance.ClearGrid();
            gameplayPanel.SetActive(false);
            levelCompletedPanel.SetActive(true);
        }
    }
}
