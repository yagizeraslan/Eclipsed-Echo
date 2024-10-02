using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YagizEraslan.EclipsedEcho
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown categoryDropdown;
        [SerializeField] private TMP_Dropdown layoutDropdown;
        [SerializeField] private Button playButton;

        private LevelManager levelManager;
        private List<int> availableGridSizes = new List<int>();
        private List<CardCategory> cardCategories;

        public List<CardCategory> CardCategories => cardCategories;
        public TMP_Dropdown CategoryDropdown => categoryDropdown;

        public static MainMenuManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            levelManager = LevelManager.Instance;
            playButton.onClick.AddListener(OnPlayButtonClicked);


            PopulateDropdowns();
        }

        private void PopulateDropdowns()
        {
            // Populate dropdowns initially or when data changes
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

            availableGridSizes = LevelManager.Instance.GenerateGridSizeOptions(minColumns, maxColumns, maxRows, gridRect, spacing);

            layoutDropdown.ClearOptions();
            layoutDropdown.AddOptions(availableGridSizes.ConvertAll(size => size.ToString()));
        }

        private void OnPlayButtonClicked()
        {
            DataPersistenceManager.Instance.SaveSelectedLevelType(availableGridSizes[layoutDropdown.value], categoryDropdown.value);
            LevelManager.Instance.GenerateLevel(availableGridSizes[layoutDropdown.value], categoryDropdown.value);
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
        }
    }
}
