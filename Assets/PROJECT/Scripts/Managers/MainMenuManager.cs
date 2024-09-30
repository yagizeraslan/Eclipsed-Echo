using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YagizEraslan.EclipsedEcho
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown layoutDropdown;
        [SerializeField] private TMP_Dropdown categoryDropdown;
        [SerializeField] private Button playButton;

        private LevelManager levelManager;
        private List<int> availableGridSizes = new List<int>();
        private List<CardCategory> cardCategories;

        private void Start()
        {
            levelManager = LevelManager.Instance;
            playButton.onClick.AddListener(OnPlayButtonClicked);

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
            GridLayoutGroup gridLayoutGroup = levelManager.GetGridLayoutGroup();

            float gridWidth = gridLayoutGroup.GetComponent<RectTransform>().rect.width;
            float gridHeight = gridLayoutGroup.GetComponent<RectTransform>().rect.height;

            float spacingX = gridLayoutGroup.spacing.x;
            float spacingY = gridLayoutGroup.spacing.y;

            int minColumns = 2;
            int maxColumns = Mathf.FloorToInt((gridWidth + spacingX) / (levelManager.MinCellSize + spacingX));

            List<int> totalCardOptions = new List<int>();

            for (int columns = minColumns; columns <= maxColumns; columns++)
            {
                // Calculate the maximum number of rows for the current number of columns
                float cellWidth = (gridWidth - ((columns - 1) * spacingX)) / columns;

                // Skip if cell width is less than minimum card size
                if (cellWidth < levelManager.MinCellSize)
                    continue;

                int maxRows = Mathf.FloorToInt((gridHeight + spacingY) / (levelManager.MinCellSize + spacingY));

                for (int rows = 2; rows <= maxRows; rows++)
                {
                    float cellHeight = (gridHeight - ((rows - 1) * spacingY)) / rows;

                    // Skip if cell height is less than minimum card size
                    if (cellHeight < levelManager.MinCellSize)
                        continue;

                    int totalCards = columns * rows;

                    // Ensure totalCards is even (so every card has a pair)
                    if (totalCards % 2 != 0)
                        continue;

                    // Avoid duplicates
                    if (totalCardOptions.Contains(totalCards))
                        continue;

                    totalCardOptions.Add(totalCards);
                }
            }

            totalCardOptions.Sort();

            layoutDropdown.ClearOptions();
            List<string> options = totalCardOptions.ConvertAll(size => size.ToString());
            layoutDropdown.AddOptions(options);

            // Store available grid sizes for use when starting the game
            availableGridSizes = totalCardOptions;
        }

        private void OnPlayButtonClicked()
        {
            int selectedLayoutIndex = layoutDropdown.value;
            int selectedGridSize = availableGridSizes[selectedLayoutIndex];

            int selectedCategoryIndex = categoryDropdown.value;
            CardCategory selectedCategory = cardCategories[selectedCategoryIndex];

            levelManager.SetSelectedGridSize(selectedGridSize);
            levelManager.SetSelectedCardCategory(selectedCategory);
            GameManager.Instance.StartGame();
        }
    }
}