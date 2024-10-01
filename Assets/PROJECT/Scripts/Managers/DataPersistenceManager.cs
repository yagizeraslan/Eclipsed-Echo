using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    public class DataPersistenceManager : MonoSingleton<DataPersistenceManager>
    {
        private const string SELECTED_GRID_KEY = "SelectedGridType";
        private const string SELECTED_CATEGORY_KEY = "SelectedCategoryType";
        private const string HIGH_SCORE_KEY = "HighScore";
        private const string GAME_STATE_KEY = "GameState";

        public int SelectedGridSize => PlayerPrefs.GetInt(SELECTED_GRID_KEY);
        public int SelectedCategoryKey => PlayerPrefs.GetInt(SELECTED_CATEGORY_KEY);

        #region PlayerPrefs Utilities

        private void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        private int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : defaultValue;
        }

        private void SaveString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        private string LoadString(string key, string defaultValue = "")
        {
            return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : defaultValue;
        }

        private void DeleteKey(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
            }
        }

        #endregion

        #region Save/Load/Delete Level Data

        public void SaveSelectedLevelType(int selectedGridSize, int selectedCategoryKey)
        {
            SaveInt(SELECTED_GRID_KEY, selectedGridSize);
            SaveInt(SELECTED_CATEGORY_KEY, selectedCategoryKey);
        }

        public bool LoadSelectedLevelType(out int selectedGridSize, out int selectedCategoryKey)
        {
            if (PlayerPrefs.HasKey(SELECTED_GRID_KEY) && PlayerPrefs.HasKey(SELECTED_CATEGORY_KEY))
            {
                selectedGridSize = LoadInt(SELECTED_GRID_KEY);
                selectedCategoryKey = LoadInt(SELECTED_CATEGORY_KEY);
                return true;
            }

            selectedGridSize = 0;
            selectedCategoryKey = 0;
            return false;
        }

        public void DeleteSelectedLevelData()
        {
            DeleteKey(SELECTED_GRID_KEY);
            DeleteKey(SELECTED_CATEGORY_KEY);
        }

        #endregion

        #region Save/Load High Score

        public void SaveHighScore(int highScore)
        {
            int currentHighScore = LoadInt(HIGH_SCORE_KEY);
            if (highScore > currentHighScore)
            {
                SaveInt(HIGH_SCORE_KEY, highScore);
            }
        }

        public int LoadHighScore()
        {
            return LoadInt(HIGH_SCORE_KEY);
        }

        public void DeleteHighScore()
        {
            DeleteKey(HIGH_SCORE_KEY);
        }

        #endregion

        #region Save/Load Game State

        public void SaveGameState(List<CardData> cardData, int score, float timer, int turns, int matches)
        {
            GameState gameState = new GameState
            {
                cardData = cardData,
                score = score,
                timer = timer,
                turns = turns,
                matches = matches
            };

            string json = JsonUtility.ToJson(gameState);
            SaveString(GAME_STATE_KEY, json);
        }

        public GameState LoadGameState()
        {
            if (PlayerPrefs.HasKey(GAME_STATE_KEY))
            {
                string json = LoadString(GAME_STATE_KEY);
                return JsonUtility.FromJson<GameState>(json);
            }
            return null;
        }

        public void ClearSavedGame()
        {
            DeleteKey(GAME_STATE_KEY);
        }

        public bool HasSavedGame()
        {
            return PlayerPrefs.HasKey(GAME_STATE_KEY);
        }

        #endregion

        #region Level Load/Resume Actions

        public void LoadLevel()
        {
            if (HasSavedGame())
            {
                LoadGameState();
                GameManager.Instance.ShowGameplayPanel();
                GameManager.Instance.ResumeGame();
            }
        }

        public void DeclineLoadLevel()
        {
            ClearSavedGame();
            GameManager.Instance.ShowMainMenuPanel();
        }

        #endregion
    }

    [System.Serializable]
    public class GameState
    {
        public List<CardData> cardData;
        public int score;
        public float timer;
        public int turns;
        public int matches;
    }
}
