using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YagizEraslan.EclipsedEcho
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [SerializeField] private GridLayoutGroup cardGridLayoutGroup;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private BackSideCards backSideCards;
        [SerializeField] private List<CardCategory> cardCategories;

        private CardCategory selectedCardCategory;
        private List<Card> cards = new List<Card>();
        private int selectedGridSize;
        private string selectedBackSideSpriteAddress;

        private float minCellSize = 300f, maxCellSize = 500f;
        public float MinCellSize => minCellSize;
        public float MaxCellSize => maxCellSize;


        public List<CardCategory> GetCardCategories()
        {
            return cardCategories;
        }

        public int TotalPairs { get; private set; }

        private void Start()
        {
            GameManager.Instance.OnGameStart += SetupLevel;
        }

        public void SetSelectedCardCategory(CardCategory category)
        {
            selectedCardCategory = category;
        }

        public void SetSelectedGridSize(int gridSize)
        {
            selectedGridSize = gridSize;
        }

        private void SetupLevel()
        {
            AdjustGridLayout();
            SpawnCardsAsync();
        }

        private void AdjustGridLayout()
        {
            int totalCards = selectedGridSize;

            // Find the best fit for columns and rows
            int columns = 0;
            int rows = 0;
            float cellSize = 0f;

            GridLayoutGroup gridLayoutGroup = cardGridLayoutGroup;
            float gridWidth = gridLayoutGroup.GetComponent<RectTransform>().rect.width;
            float gridHeight = gridLayoutGroup.GetComponent<RectTransform>().rect.height;
            float spacingX = gridLayoutGroup.spacing.x;
            float spacingY = gridLayoutGroup.spacing.y;

            // Loop over possible columns starting from 2 up to totalCards
            for (int c = 2; c <= totalCards; c++)
            {
                int r = Mathf.CeilToInt((float)totalCards / c);

                // Calculate cell size
                float cellWidth = (gridWidth - ((c - 1) * spacingX)) / c;
                float cellHeight = (gridHeight - ((r - 1) * spacingY)) / r;
                float currentCellSize = Mathf.Min(cellWidth, cellHeight);

                if (currentCellSize >= minCellSize)
                {
                    // Update if this configuration gives a larger cell size
                    if (currentCellSize > cellSize)
                    {
                        cellSize = currentCellSize;
                        columns = c;
                        rows = r;
                    }
                }
            }

            // Clamp cell size
            cellSize = Mathf.Clamp(cellSize, minCellSize, maxCellSize);

            // Set grid layout properties
            cardGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            cardGridLayoutGroup.constraintCount = columns;
            cardGridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        }

        private async void SpawnCardsAsync()
        {
            // Randomly select a back-side sprite for this game
            selectedBackSideSpriteAddress = backSideCards.backSideSpriteAddresses[Random.Range(0, backSideCards.backSideSpriteAddresses.Count)];

            // Use the selected card category's sprite addresses
            List<string> frontSideCardSpritesAddressables = selectedCardCategory.cardSpriteAddresses;

            // Generate pairs of card IDs
            List<int> cardIDs = new List<int>();
            int totalPairs = selectedGridSize / 2;

            TotalPairs = totalPairs;

            for (int i = 0; i < totalPairs; i++)
            {
                cardIDs.Add(i);
                cardIDs.Add(i);
            }

            // Shuffle the card IDs
            cardIDs = ShuffleList(cardIDs);

            for (int i = 0; i < cardIDs.Count; i++)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardGridLayoutGroup.transform);
                Card card = cardObj.GetComponent<Card>();
                cards.Add(card);

                int cardID = cardIDs[i];
                string frontSpriteAddress = frontSideCardSpritesAddressables[cardID % frontSideCardSpritesAddressables.Count];

                await card.Initialize(cardID, frontSpriteAddress, selectedBackSideSpriteAddress);

                // Delay for spawn animation
                await Task.Delay(100);

                // Flip to show front side
                card.FlipCard();
                SoundManager.Instance.PlayFlipSound();

                // Delay before flipping back
                await Task.Delay(500);

                // Flip back to show back side
                card.FlipCard();
                SoundManager.Instance.PlayFlipSound();
            }
        }

        private List<T> ShuffleList<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
            return list;
        }

        // Additional method to provide GridLayoutGroup reference
        public GridLayoutGroup GetGridLayoutGroup()
        {
            return cardGridLayoutGroup;
        }
    }
}
