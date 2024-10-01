using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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

        [SerializeField] private float minCellSize = 300f, maxCellSize = 500f;
        public float MinCellSize => minCellSize;
        public float MaxCellSize => maxCellSize;

        public List<CardCategory> GetCardCategories()
        {
            return cardCategories;
        }

        public List<CardData> GetCardsData()
        {
            List<CardData> cardDataList = new List<CardData>();
            foreach (var card in cards)
            {
                cardDataList.Add(new CardData
                {
                    cardID = card.CardID,
                    isFlipped = card.IsFaceUp,
                    isMatched = card.IsMatched
                });
            }
            return cardDataList;
        }

        public void LoadCardsFromState(List<CardData> cardDataList)
        {
            for (int i = 0; i < cardDataList.Count; i++)
            {
                CardData cardData = cardDataList[i];
                Card card = cards.FirstOrDefault(c => c.CardID == cardData.cardID);
                if (card != null)
                {
                    if (cardData.isFlipped)
                    {
                        card.FlipToFrontSide();
                    }
                    if (cardData.isMatched)
                    {
                        card.Match();
                    }
                }
            }
        }

        public int TotalPairs { get; private set; }

        private void Start()
        {
            GameManager.Instance.OnGameStart += SetupLevel;
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

        private void SetupLevel()
        {
            AdjustGridLayout();
            SpawnCardsAsync();
        }

        private void AdjustGridLayout()
        {
            int totalCards = selectedGridSize;

            (int columns, int rows, float cellSize) = CalculateOptimalGridDimensions(totalCards);

            cellSize = Mathf.Clamp(cellSize, minCellSize, maxCellSize);

            ApplyGridLayoutProperties(columns, cellSize);
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

        private async Task SpawnCardsAsync()
        {
            selectedBackSideSpriteAddress = backSideCards.backSideSpriteAddresses[UnityEngine.Random.Range(0, backSideCards.backSideSpriteAddresses.Count)];

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

            cards = new List<Card>();
            for (int i = 0; i < cardIDs.Count; i++)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardGridLayoutGroup.transform);
                Card card = cardObj.GetComponent<Card>();
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

        public void RestartCurrentLevel()
        {
            SetupLevel();
        }

        public void ClearGrid()
        {
            foreach (Transform child in cardGridLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public GridLayoutGroup GetGridLayoutGroup()
        {
            return cardGridLayoutGroup;
        }
    }
}