using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    public class DataPersistenceManager : MonoSingleton<DataPersistenceManager>
    {
        private const string SelectedGridKey = "SelectedGridType";
        private const string SelectedCategoryKey = "SelectedCategoryType";
        private const string HighScoreKey = "HighScore";

        #region Save/Load/Delete Level Data

        public void SaveSelectedGridType(int selectedGridSize, string selectedCategoryName)
        {
            PlayerPrefs.SetInt(SelectedGridKey, selectedGridSize);
            PlayerPrefs.SetString(SelectedCategoryKey, selectedCategoryName);
            PlayerPrefs.Save();
        }

        public bool LoadSelectedGridType(out int selectedGridSize, out string selectedCategoryName)
        {
            if (PlayerPrefs.HasKey(SelectedGridKey) && PlayerPrefs.HasKey(SelectedCategoryKey))
            {
                selectedGridSize = PlayerPrefs.GetInt(SelectedGridKey);
                selectedCategoryName = PlayerPrefs.GetString(SelectedCategoryKey);
                return true;
            }
            else
            {
                selectedGridSize = 0;
                selectedCategoryName = string.Empty;
                return false;
            }
        }

        public void DeleteSelectedGridType()
        {
            PlayerPrefs.DeleteKey(SelectedGridKey);
            PlayerPrefs.DeleteKey(SelectedCategoryKey);
            PlayerPrefs.Save();
        }

        #endregion

        #region Save/Load High Score
        public void SaveHighScore(int highScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        public int LoadHighScore()
        {
            if (PlayerPrefs.HasKey(HighScoreKey))
            {
                return PlayerPrefs.GetInt(HighScoreKey);
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}