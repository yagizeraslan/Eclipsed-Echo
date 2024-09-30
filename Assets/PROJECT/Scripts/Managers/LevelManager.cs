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

        private async Task SpawnCardsAsync()
        {
            GameController.Instance.SetProcessing(true); // Prevent interaction during initial reveal

            // Randomly select a back-side sprite for this game
            selectedBackSideSpriteAddress = backSideCards.backSideSpriteAddresses[UnityEngine.Random.Range(0, backSideCards.backSideSpriteAddresses.Count)];

            // Use the selected card category's sprite addresses
            List<string> frontSideCardSpritesAddressables = new List<string>(selectedCardCategory.cardSpriteAddresses);

            // Generate pairs of card IDs
            List<int> cardIDs = new List<int>();
            int totalPairs = selectedGridSize / 2;

            TotalPairs = totalPairs;

            for (int i = 0; i < totalPairs; i++)
            {
                cardIDs.Add(i);
                cardIDs.Add(i); // Add each card ID twice for matching pairs
            }

            // Shuffle the card IDs
            cardIDs = ShuffleList(cardIDs);

            // Shuffle the front side sprites independently as well
            frontSideCardSpritesAddressables = ShuffleList(frontSideCardSpritesAddressables);

            // Instantiate and initialize all cards
            cards = new List<Card>();
            for (int i = 0; i < cardIDs.Count; i++)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardGridLayoutGroup.transform);
                Card card = cardObj.GetComponent<Card>();
                cards.Add(card);

                int cardID = cardIDs[i]; // Get the shuffled card ID
                string frontSpriteAddress = frontSideCardSpritesAddressables[cardID % frontSideCardSpritesAddressables.Count];

                await card.Initialize(cardID, frontSpriteAddress, selectedBackSideSpriteAddress);
            }

            // Start the cascading flip sequence
            List<Task> flipTasks = new List<Task>();
            float delayBetweenCards = 100f; // 100 milliseconds between each card's flip
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                float delay = i * delayBetweenCards;
                flipTasks.Add(ShowAndFlipCard(card, delay));
            }

            // Wait for all flip sequences to complete
            await Task.WhenAll(flipTasks);

            // Enable interaction on all cards
            foreach (var card in cards)
            {
                card.SetInteractable(true);
            }

            GameController.Instance.SetProcessing(false); // Allow player interaction
        }

        private async Task ShowAndFlipCard(Card card, float delay)
        {
            await Task.Delay((int)delay);
            card.ShowCard();
            await Task.Delay(500);
            card.FlipToFrontSide();
            await Task.Delay(1500);
            card.FlipToBackSide();
            await Task.Delay(500);
        }

        private List<T> ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
            return list;
        }

        public GridLayoutGroup GetGridLayoutGroup()
        {
            return cardGridLayoutGroup;
        }
    }
}