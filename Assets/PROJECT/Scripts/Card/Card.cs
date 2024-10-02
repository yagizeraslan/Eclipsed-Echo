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

        // Check if the card can be clicked based on its current state
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

            // Reset the card to its initial state
            ResetCard();
        }

        // Set the card's interactable state
        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
        }

        // Handle pointer click event for flipping the card
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsClickable()) return;
            GameController.Instance.CardSelected(this);
        }

        // Flip the card to its front side with animation
        public void FlipToFrontSide()
        {
            if (!IsFaceUp && !IsMatched)
            {
                animator.SetTrigger("FlipFrontSide");
                SoundManager.Instance.PlayFlipSound();
                IsFaceUp = true;
            }
        }

        // Flip the card to its back side with animation
        public void FlipToBackSide()
        {
            if (IsFaceUp && !IsMatched)
            {
                animator.SetTrigger("FlipBackSide");
                SoundManager.Instance.PlayFlipSound();
                IsFaceUp = false;
            }
        }

        // Mark the card as matched and play the match animation
        public void Match()
        {
            IsMatched = true;
            IsFaceUp = true;
            animator.SetTrigger("Match");
            SoundManager.Instance.PlayMatchSound();
        }

        // Mark the card as mismatched and play the mismatch animation
        public void Mismatch()
        {
            animator.SetTrigger("Mismatch");
            SoundManager.Instance.PlayMismatchSound();
            IsFaceUp = false;
        }

        // Reset the card's state and prepare it for reuse (for object pooling)
        public void ResetCard()
        {
            // Reset card state
            IsMatched = false;
            IsFaceUp = false;
            SetInteractable(false);

            // Reset the animator to its default state, clear all triggers
            animator.Rebind();
            animator.Update(0);

            // Ensure the card shows the back side and hides the front side
            backImage.gameObject.SetActive(true);
            frontImage.gameObject.SetActive(false);

            // Reset the card's scale in case animations changed it
            transform.localScale = Vector3.one;
        }

        // Manually trigger showing the card (could be used for intro animations)
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

        // This method can be called by an animation event at the end of HideCard animation
        public void OnHideCardAnimationEnd()
        {
            // Logic for handling the card when its hide animation is finished
            Debug.Log($"Card {CardID} has completed HideCard animation.");
            IsMatched = true; // Ensure the card is marked as matched after hiding
        }
    }
}
