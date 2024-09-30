using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;


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

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public async Task Initialize(int cardID, string frontSpriteAddress, string backSpriteAddress)
        {
            CardID = cardID;

            // Load front sprite
            var frontSpriteLoad = Addressables.LoadAssetAsync<Sprite>(frontSpriteAddress);
            await frontSpriteLoad.Task;
            frontImage.sprite = frontSpriteLoad.Result;

            // Load back sprite
            var backSpriteLoad = Addressables.LoadAssetAsync<Sprite>(backSpriteAddress);
            await backSpriteLoad.Task;
            backImage.sprite = backSpriteLoad.Result;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsMatched || GameController.Instance.IsProcessing)
                return;

            FlipCard();
            GameController.Instance.CardSelected(this);
        }

        public void FlipCard()
        {
            animator.SetTrigger("Flip");
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

        public void HideCard()
        {
            gameObject.SetActive(false);
        }
    }
}