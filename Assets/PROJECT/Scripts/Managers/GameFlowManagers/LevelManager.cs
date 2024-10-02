using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace YagizEraslan.EclipsedEcho
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup cardGridLayoutGroup;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private BackSideCards backSideCards;
        [SerializeField] private List<CardCategory> cardCategories;
        [SerializeField] private int poolSize = 60; // You can adjust this based on your game

        private CardCategory selectedCardCategory;
        private List<Card> cards = new List<Card>();
        private int selectedGridSize;
        private string selectedBackSideSpriteAddress;

        private ObjectPool<Card> cardPool;

        [SerializeField] private float minCellSize = 400f, maxCellSize = 500f;

        public float MinCellSize => minCellSize;
        public float MaxCellSize => maxCellSize;

        public List<CardCategory> GetCardCategories() { return cardCategories; }

        public int TotalPairs { get; private set; }

        public static LevelManager Instance;

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
            GameManager.Instance.OnGameStart += SetupLevel;
            InitializeCardPool();
        }


        #region Card Management
        public List<CardData> GetCardsData()
        {
            List<CardData> cardDataList = new List<CardData>();
            foreach (var card in cards)
            {
                cardDataList.Add(new CardData
                {
                    CardID = card.CardID,
                    IsFlipped = card.IsFaceUp,
                    IsMatched = card.IsMatched
                });
            }
            return cardDataList;
        }

        private async Task SpawnCardsAsync()
        {
            selectedBackSideSpriteAddress = backSideCards.backSideSpriteAddresses[Random.Range(0, backSideCards.backSideSpriteAddresses.Count)];

            List<string> frontSideCardSpritesAddressables = new List<string>(selectedCardCategory.cardSpriteAddresses);

            List<int> cardIDs = new List<int>();
            int totalPairs = selectedGridSize / 2;

            TotalPairs = totalPairs;

            for (int i = 0; i < totalPairs; i++)
            {
                cardIDs.Add(i);
                cardIDs.Add(i);
            }

            cardIDs = ShuffleList(cardIDs);
            frontSideCardSpritesAddressables = ShuffleList(frontSideCardSpritesAddressables);

            // Deactivate all active cards before spawning new ones
            foreach (var card in cards)
            {
                cardPool.ReturnObjectToPool(card);
            }
            cards.Clear();

            // Spawn cards from the pool
            for (int i = 0; i < cardIDs.Count; i++)
            {
                Card card = cardPool.GetObjectFromPool();
                card.ResetCard(); // Ensure the card is reset before use
                cards.Add(card);

                int cardID = cardIDs[i];
                string frontSpriteAddress = frontSideCardSpritesAddressables[cardID % frontSideCardSpritesAddressables.Count];

                await card.Initialize(cardID, frontSpriteAddress, selectedBackSideSpriteAddress);
            }

            // Start the cascading flip sequence
            List<Task> flipTasks = new List<Task>();
            float delayBetweenCards = 100f;
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                float delay = i * delayBetweenCards;
                flipTasks.Add(ShowAndFlipCard(card, delay));
            }

            await Task.WhenAll(flipTasks);

            foreach (var card in cards)
            {
                card.SetInteractable(true);
            }

            TimerManager.Instance.StartTimer();
        }

        public void LoadCardsFromState(List<CardData> cardDataList)
        {
            for (int i = 0; i < cardDataList.Count; i++)
            {
                CardData cardData = cardDataList[i];
                Card card = cards.FirstOrDefault(c => c.CardID == cardData.CardID);
                if (card != null)
                {
                    if (cardData.IsFlipped)
                    {
                        card.FlipToFrontSide();
                    }
                    if (cardData.IsMatched)
                    {
                        card.Match();
                    }
                }
            }
        }

        public void ClearGrid()
        {
            foreach (Transform child in cardGridLayoutGroup.transform)
            {
                Card card = child.GetComponent<Card>();
                if (card != null)
                {
                    cardPool.ReturnObjectToPool(card);
                }
            }

            cards.Clear(); // Clear the cards list
        }
        private void InitializeCardPool()
        {
            cardPool = new ObjectPool<Card>(cardPrefab, poolSize, cardGridLayoutGroup.transform);
        }
        #endregion


        #region Level Management
        public void GenerateLevel(int gridSize, int cardCategoryIndex)
        {
            ScoreManager.Instance.InitializeStartingValues();
            TimerManager.Instance.ResetTimer();

            if (!DataPersistenceManager.Instance.HasSavedGame())
            {
                SetSelectedGridSize(gridSize);
                SetSelectedCardCategory(cardCategoryIndex);
            }
            else
            {
                SetSelectedGridSize(DataPersistenceManager.Instance.SelectedGridSize);
                SetSelectedCardCategory(DataPersistenceManager.Instance.SelectedCategoryKey);
            }
            GameManager.Instance.ShowGameplayPanel();
        }

        private void SetupLevel()
        {
            cardPool.ResetPoolIndex();
            AdjustGridLayout();
            SpawnCardsAsync();
        }

        public void SetSelectedCardCategory(int index)
        {
            if (index >= 0 && index < cardCategories.Count)
            {
                selectedCardCategory = cardCategories[index];
            }
            else
            {
                Debug.LogError("Invalid index for card category.");
            }
        }

        public void SetSelectedGridSize(int gridSize)
        {
            selectedGridSize = gridSize;
        }
        #endregion


        #region Grid Layout
        private void AdjustGridLayout()
        {
            int totalCards = selectedGridSize;

            (int columns, int rows, float cellSize) = CalculateOptimalGridDimensions(totalCards);

            cellSize = Mathf.Clamp(cellSize, minCellSize, maxCellSize);

            ApplyGridLayoutProperties(columns, cellSize);
        }

        public List<int> GenerateGridSizeOptions(int minColumns, int maxColumns, int maxRows, Rect gridRect, Vector2 spacing)
        {
            var totalCardOptions = new List<int>();

            for (int columns = minColumns; columns <= maxColumns; columns++)
            {
                float cellWidth = (gridRect.width - (columns - 1) * spacing.x) / columns;
                if (cellWidth < minCellSize) continue;

                for (int rows = 2; rows <= maxRows; rows++)
                {
                    float cellHeight = (gridRect.height - (rows - 1) * spacing.y) / rows;
                    if (cellHeight < maxCellSize) continue;

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

        private (int columns, int rows, float cellSize) CalculateOptimalGridDimensions(int totalCards)
        {
            int columns = 0;
            int rows = 0;
            float cellSize = 0f;

            GridLayoutGroup gridLayoutGroup = cardGridLayoutGroup;
            float gridWidth = gridLayoutGroup.GetComponent<RectTransform>().rect.width;
            float gridHeight = gridLayoutGroup.GetComponent<RectTransform>().rect.height;
            float spacingX = gridLayoutGroup.spacing.x;
            float spacingY = gridLayoutGroup.spacing.y;

            for (int c = 2; c <= totalCards; c++)
            {
                int r = Mathf.CeilToInt((float)totalCards / c);

                float currentCellSize = CalculateCellSize(c, r, gridWidth, gridHeight, spacingX, spacingY);

                if (currentCellSize >= minCellSize && currentCellSize > cellSize)
                {
                    cellSize = currentCellSize;
                    columns = c;
                    rows = r;
                }
            }

            return (columns, rows, cellSize);
        }

        private float CalculateCellSize(int columns, int rows, float gridWidth, float gridHeight, float spacingX, float spacingY)
        {
            float cellWidth = (gridWidth - ((columns - 1) * spacingX)) / columns;
            float cellHeight = (gridHeight - ((rows - 1) * spacingY)) / rows;

            return Mathf.Min(cellWidth, cellHeight);
        }

        private void ApplyGridLayoutProperties(int columns, float cellSize)
        {
            cardGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            cardGridLayoutGroup.constraintCount = columns;
            cardGridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        }

        public GridLayoutGroup GetGridLayoutGroup()
        {
            return cardGridLayoutGroup;
        }
        #endregion


        #region Utilitty
        private List<T> ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
            return list;
        }

        private const int SHOW_DELAY = 500;
        private const int FLIP_DELAY = 1500;
        private const int HIDE_DELAY = 500;

        private async Task ShowAndFlipCard(Card card, float delay)
        {
            await Task.Delay((int)delay);
            card.ShowCard();
            await Task.Delay(SHOW_DELAY);
            card.FlipToFrontSide();
            await Task.Delay(FLIP_DELAY);
            card.FlipToBackSide();
            await Task.Delay(HIDE_DELAY);
        }
        #endregion
    }
}
