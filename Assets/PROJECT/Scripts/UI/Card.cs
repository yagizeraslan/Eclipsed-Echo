using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace YagizEraslan.EclipsedEcho
{
    [RequireComponent(typeof(Animator))]
    public class Card : MonoBehaviour, IPointerClickHandler
    {
        public int CardID { get; private set; }
        public bool IsMatched { get; private set; }
        public bool IsFaceUp { get; private set; }

        private bool isMismatchedAnimationComplete = false;

        [SerializeField] private Image frontImage;
        [SerializeField] private Image backImage;

        private Animator animator;
        private bool isInteractable = false;

        private AsyncOperationHandle<Sprite> frontSpriteHandle;
        private AsyncOperationHandle<Sprite> backSpriteHandle;

        public UnityAction<Card> OnFlipComplete;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            IsFaceUp = false;
        }

        public async Task Initialize(int cardID, string frontSpriteAddress, string backSpriteAddress)
        {
            CardID = cardID;

            // Load sprites asynchronously and store their handles
            frontSpriteHandle = Addressables.LoadAssetAsync<Sprite>(frontSpriteAddress);
            await frontSpriteHandle.Task;
            frontImage.sprite = frontSpriteHandle.Result;

            backSpriteHandle = Addressables.LoadAssetAsync<Sprite>(backSpriteAddress);
            await backSpriteHandle.Task;
            backImage.sprite = backSpriteHandle.Result;

            // Start in ResetCard state
            animator.Play("ResetCard", -1, 0f);
        }

        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isInteractable || IsMatched || IsFaceUp)
                return;

            GameController.Instance.CardSelected(this);
        }


        public void ShowCard()
        {
            animator.SetTrigger("ShowCard");
        }

        public void HideCard()
        {
            animator.SetTrigger("HideCard");
        }

        public void FlipToFrontSide()
        {
            if (!IsFaceUp)
            {
                animator.SetTrigger("FlipFrontSide");
                SoundManager.Instance.PlayFlipSound();
                IsFaceUp = true; // Set the card as face-up after flipping
            }
        }

        public void FlipToBackSide()
        {
            if (IsFaceUp)
            {
                animator.SetTrigger("FlipBackSide");
                SoundManager.Instance.PlayFlipSound();
                IsFaceUp = false; // Set the card as face-down after flipping back
            }
        }

        public void Match()
        {
            IsMatched = true; // Mark the card as matched
            IsFaceUp = true;  // Keep the card face-up
            animator.SetTrigger("Match");
            SoundManager.Instance.PlayMatchSound();
        }

        public void Mismatch()
        {
            animator.SetTrigger("Mismatch");
            SoundManager.Instance.PlayMismatchSound();
            IsFaceUp = false; // Ensure the card is considered face-down after mismatch
        }

        public void OnFlipAnimationComplete()
        {
            OnFlipComplete?.Invoke(this);
        }


        public void ReleaseAssets()
        {
            // Release front and back sprites
            if (frontSpriteHandle.IsValid())
            {
                Addressables.Release(frontSpriteHandle);
            }
            if (backSpriteHandle.IsValid())
            {
                Addressables.Release(backSpriteHandle);
            }
        }

        private void OnDestroy()
        {
            ReleaseAssets();
        }
    }
}
