using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YagizEraslan.EclipsedEcho
{
    public class MainMenuManager : MonoSingleton<MainMenuManager>
    {
        [SerializeField] private TMP_Dropdown categoryDropdown;
        [SerializeField] private TMP_Dropdown layoutDropdown;
        [SerializeField] private Button playButton;

        private LevelManager levelManager;
        private List<int> availableGridSizes = new List<int>();
        private List<CardCategory> cardCategories;

        public List<CardCategory> CardCategories => cardCategories;
        public TMP_Dropdown CategoryDropdown => categoryDropdown;

        private void Start()
        {
            levelManager = LevelManager.Instance;
            playButton.onClick.AddListener(OnPlayButtonClicked);

            PopulateDropdowns();
        }

        private void PopulateDropdowns()
        {
            PopulateLayoutDropdown();
            PopulateCategoryDropdown();
        }

        private void PopulateCategoryDropdown()
        {
            cardCategories = levelManager.GetCardCategories();
            List<string> categoryNames = cardCategories.ConvertAll(category => category.categoryName);

            categoryDropdown.ClearOptions();
            categoryDropdown.AddOptions(categoryNames);
        }

        private void PopulateLayoutDropdown()
        {
            var gridLayoutGroup = levelManager.GetGridLayoutGroup();
            var gridRect = gridLayoutGroup.GetComponent<RectTransform>().rect;
            var spacing = gridLayoutGroup.spacing;

            int minColumns = 2;
            int maxColumns = Mathf.FloorToInt((gridRect.width + spacing.x) / (levelManager.MinCellSize + spacing.x));
            int maxRows = Mathf.FloorToInt((gridRect.height + spacing.y) / (levelManager.MinCellSize + spacing.y));

            availableGridSizes = GenerateGridSizeOptions(minColumns, maxColumns, maxRows, gridRect, spacing);

            layoutDropdown.ClearOptions();
            layoutDropdown.AddOptions(availableGridSizes.ConvertAll(size => size.ToString()));
        }

        private List<int> GenerateGridSizeOptions(int minColumns, int maxColumns, int maxRows, Rect gridRect, Vector2 spacing)
        {
            var totalCardOptions = new List<int>();

            for (int columns = minColumns; columns <= maxColumns; columns++)
            {
                float cellWidth = (gridRect.width - (columns - 1) * spacing.x) / columns;
                if (cellWidth < levelManager.MinCellSize) continue;

                for (int rows = 2; rows <= maxRows; rows++)
                {
                    float cellHeight = (gridRect.height - (rows - 1) * spacing.y) / rows;
                    if (cellHeight < levelManager.MinCellSize) continue;

                    int totalCards = columns * rows;
                    if (totalCards % 2 == 0 && !totalCardOptions.Contains(totalCards))
                    {
                        totalCardOptions.Add(totalCards);
                    }
                }
            }

            totalCardOptions.Sort();
            return totalCardOptions;
        }

        private void OnPlayButtonClicked()
        {
            DataPersistenceManager.Instance.SaveSelectedLevelType(availableGridSizes[layoutDropdown.value], categoryDropdown.value);
            GenerateSelectedLevel();
        }

        public void GenerateSelectedLevel()
        {
            GameController.Instance.InitializeStartingValues();
            levelManager.SetSelectedGridSize(availableGridSizes[layoutDropdown.value]);
            levelManager.SetSelectedCardCategory(categoryDropdown.value);
            GameManager.Instance.ShowGameplayPanel();
        }

        public void GenerateCustomLevel(int gridSize, int cardCategoryIndex)
        {
            GameController.Instance.InitializeStartingValues();
            levelManager.SetSelectedGridSize(gridSize);
            levelManager.SetSelectedCardCategory(cardCategoryIndex);
            GameManager.Instance.ShowGameplayPanel();
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
        }
    }
}