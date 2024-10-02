using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace YagizEraslan.EclipsedEcho
{
    [RequireComponent(typeof(Animator))]
    public class Card : MonoBehaviour, IPointerClickHandler
    {
        public int CardID { get; private set; }
        public bool IsMatched { get; private set; }
        public bool IsFaceUp { get; private set; }

        [SerializeField] private Image frontImage;
        [SerializeField] private Image backImage;

        private Animator animator;
        private bool isInteractable = false;

        private AsyncOperationHandle<Sprite> frontSpriteHandle;
        private AsyncOperationHandle<Sprite> backSpriteHandle;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            IsFaceUp = false;
        }

        private bool IsClickable()
        {
            return isInteractable && !IsMatched && !IsFaceUp;
        }

        // Initialize the card with its front and back sprites
        public async Task Initialize(int cardID, string frontSpriteAddress, string backSpriteAddress)
        {
            CardID = cardID;

            // Load sprites asynchronously and store their handles
            frontSpriteHandle = Addressables.LoadAssetAsync<Sprite>(frontSpriteAddress);
            await frontSpriteHandle.Task;

            if (frontSpriteHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load sprite at {frontSpriteAddress}");
                return;
            }

            frontImage.sprite = frontSpriteHandle.Result;

            backSpriteHandle = Addressables.LoadAssetAsync<Sprite>(backSpriteAddress);
            await backSpriteHandle.Task;
            backImage.sprite = backSpriteHandle.Result;

            ResetCard();
        }

        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsClickable()) return;
            GameController.Instance.CardSelected(this);
        }

        public void FlipToFrontSide()
        {
            if (!IsFaceUp && !IsMatched)
            {
                animator.SetTrigger("FlipFrontSide");
                SoundManager.Instance.PlayFlipSound();
                IsFaceUp = true;
            }
        }

        public void FlipToBackSide()
        {
            if (IsFaceUp && !IsMatched)
            {
                animator.SetTrigger("FlipBackSide");
                SoundManager.Instance.PlayFlipSound();
                IsFaceUp = false;
            }
        }

        public void Match()
        {
            IsMatched = true;
            IsFaceUp = true;
            animator.SetTrigger("Match");
            SoundManager.Instance.PlayMatchSound();
        }

        public void Mismatch()
        {
            animator.SetTrigger("Mismatch");
            SoundManager.Instance.PlayMismatchSound();
            IsFaceUp = false;
        }

        // Reset the card's state and prepare it for reuse (updated for object pooling)
        public void ResetCard()
        {
            IsMatched = false;
            IsFaceUp = false;
            SetInteractable(false);

            // Reset the animator to its default state, clear all triggers
            animator.Rebind();
            animator.Update(0);

            backImage.gameObject.SetActive(true);
            frontImage.gameObject.SetActive(false);

            // Reset the card's scale in case animations changed it
            transform.localScale = Vector3.one;
        }


        public void ShowCard()
        {
            animator.SetTrigger("ShowCard");
        }

        // Clean up and release loaded sprites when the card is destroyed
        private void OnDestroy()
        {
            if (frontSpriteHandle.IsValid())
            {
                Addressables.Release(frontSpriteHandle);
            }
            if (backSpriteHandle.IsValid())
            {
                Addressables.Release(backSpriteHandle);
            }
        }
    }
}
