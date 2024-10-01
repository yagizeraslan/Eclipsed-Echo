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

        #region Save/Load/Delete Level Data

        public void SaveSelectedLevelType(int selectedGridSize, int selectedCategoryName)
        {
            PlayerPrefs.SetInt(SELECTED_GRID_KEY, selectedGridSize);
            PlayerPrefs.SetInt(SELECTED_CATEGORY_KEY, selectedCategoryName);
            PlayerPrefs.Save();
        }

        public bool LoadSelectedLevelType(out int selectedGridSize, out int selectedCategoryName)
        {
            if (PlayerPrefs.HasKey(SELECTED_GRID_KEY) && PlayerPrefs.HasKey(SELECTED_CATEGORY_KEY))
            {
                selectedGridSize = PlayerPrefs.GetInt(SELECTED_GRID_KEY);
                selectedCategoryName = PlayerPrefs.GetInt(SELECTED_CATEGORY_KEY);
                return true;
            }
            else
            {
                selectedGridSize = 0;
                selectedCategoryName = 0;
                return false;
            }
        }

        public void DeleteSelectedGridType()
        {
            PlayerPrefs.DeleteKey(SELECTED_GRID_KEY);
            PlayerPrefs.DeleteKey(SELECTED_CATEGORY_KEY);
            PlayerPrefs.Save();
        }

        #endregion

        #region Save/Load High Score

        public void SaveHighScore(int highScore)
        {
            int currentHighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY);
            if (highScore > currentHighScore) 
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
                PlayerPrefs.Save();
            }
        }

        public int LoadHighScore()
        {
            if (PlayerPrefs.HasKey(HIGH_SCORE_KEY))
            {
                return PlayerPrefs.GetInt(HIGH_SCORE_KEY);
            }
            else
            {
                return 0;
            }
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
                matches = matches,
            };

            string json = JsonUtility.ToJson(gameState);
            PlayerPrefs.SetString(GAME_STATE_KEY, json);
            PlayerPrefs.Save();
        }

        public void LoadLevel()
        {
            LoadGameState();
            GameManager.Instance.ShowGameplayPanel();
            GameManager.Instance.ResumeGame();
        }

        public void DeclineLoadLevel()
        {
            ClearSavedGame();
            GameManager.Instance.ShowMainMenuPanel();
        }

        public GameState LoadGameState()
        {
            if (PlayerPrefs.HasKey(GAME_STATE_KEY))
            {
                string json = PlayerPrefs.GetString(GAME_STATE_KEY);
                return JsonUtility.FromJson<GameState>(json);
            }
            return null;
        }

        public void ClearSavedGame()
        {
            PlayerPrefs.DeleteKey(GAME_STATE_KEY);
            PlayerPrefs.Save();
        }

        public bool HasSavedGame()
        {
            return PlayerPrefs.HasKey(GAME_STATE_KEY);
        }

        #endregion
    }

    [System.Serializable]
    public class CardData
    {
        public int cardID;
        public bool isFlipped;
        public bool isMatched;
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
