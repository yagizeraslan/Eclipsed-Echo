using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Events;


namespace YagizEraslan.EclipsedEcho
{
    [RequireComponent(typeof(Animator))]
    public class Card : MonoBehaviour, IPointerClickHandler
    {
        public int CardID { get; private set; }
        public bool IsMatched { get; private set; }

        [SerializeField] private Image frontImage;
        [SerializeField] private Image backImage;

        private Animator animator;

        // Event to notify when the flip animation completes
        public UnityAction<Card> OnFlipComplete;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public async Task Initialize(int cardID, string frontSpriteAddress, string backSpriteAddress)
        {
            CardID = cardID;

            // Load sprites asynchronously
            var frontSpriteLoad = Addressables.LoadAssetAsync<Sprite>(frontSpriteAddress);
            await frontSpriteLoad.Task;
            frontImage.sprite = frontSpriteLoad.Result;

            var backSpriteLoad = Addressables.LoadAssetAsync<Sprite>(backSpriteAddress);
            await backSpriteLoad.Task;
            backImage.sprite = backSpriteLoad.Result;

            // Start in ResetCard state
            animator.Play("ResetCard", -1, 0f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsMatched || GameController.Instance.IsProcessing)
                return;

            FlipToFrontSide();
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
            animator.SetTrigger("FlipFrontSide");
            SoundManager.Instance.PlayFlipSound();
        }

        public void FlipToBackSide()
        {
            animator.SetTrigger("FlipBackSide");
            SoundManager.Instance.PlayFlipSound();
        }

        public void Match()
        {
            IsMatched = true;
            animator.SetTrigger("Match");
            SoundManager.Instance.PlayMatchSound();
        }

        public void Mismatch()
        {
            animator.SetTrigger("Mismatch");
            SoundManager.Instance.PlayMismatchSound();
        }

        // This method is called by the animation event at the end of the FlipFrontSide animation
        public void OnFlipAnimationComplete()
        {
            OnFlipComplete?.Invoke(this);
        }
    }
}