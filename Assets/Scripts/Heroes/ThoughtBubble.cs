using System;
using DG.Tweening;
using Items;
using NaughtyAttributes;
using UnityEngine;

namespace Heroes {

    public enum ThoughtBubbleAnimation {
        None, Mad, Happy, Potion, Sword, Book
    }
    public class ThoughtBubble: MonoBehaviour {
        private static readonly int EmotionIndex = Animator.StringToHash("EmotionIndex");

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer itemBubbleRenderer;
        [SerializeField] private SpriteRenderer itemRenderer;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private Sprite potionSprite;
        [SerializeField] private Sprite swordSprite;
        [SerializeField] private Sprite bookSprite;
        [SerializeField] private Animator animator;

        private ThoughtBubbleAnimation _thoughtBubbleAnimation = ThoughtBubbleAnimation.None;

        public ThoughtBubbleAnimation thoughtBubbleAnimation {
            get => this._thoughtBubbleAnimation;
            set {
                if (value != this._thoughtBubbleAnimation) {
                    this._thoughtBubbleAnimation = value;
                    this.SetAnimation();
                }
            }
        }

        public void SetAnimationWithTime(Item item, float time) {
            switch (item) {
                case Item.Potion: {
                    this._thoughtBubbleAnimation = ThoughtBubbleAnimation.Potion;
                    break;
                }
                case Item.Sword: {
                    this._thoughtBubbleAnimation = ThoughtBubbleAnimation.Sword;
                    break;
                }
                case Item.SpellBook: {
                    this._thoughtBubbleAnimation = ThoughtBubbleAnimation.Book;
                    break;
                }
            }
            this.SetAnimation();
            this.itemContainer.DOScale(0.001f, time);
        }
        private void Awake() {
            this.spriteRenderer.gameObject.SetActive(false);
        }

        private void SetAnimation() {
            this.itemContainer.DOKill();
            if (this._thoughtBubbleAnimation == ThoughtBubbleAnimation.Book ||
                this._thoughtBubbleAnimation == ThoughtBubbleAnimation.Potion ||
                this._thoughtBubbleAnimation == ThoughtBubbleAnimation.Sword) {
                this.itemContainer.localScale = new Vector3(1f,1f,1f);
                this.animator.SetInteger(EmotionIndex, 0);
                this.spriteRenderer.gameObject.SetActive(false);
                this.spriteRenderer.sprite = null;
                
                this.itemContainer.gameObject.SetActive(true);

                if (this._thoughtBubbleAnimation == ThoughtBubbleAnimation.Book) {
                    this.itemRenderer.sprite = this.bookSprite;
                } else if (this._thoughtBubbleAnimation == ThoughtBubbleAnimation.Potion) {
                    this.itemRenderer.sprite = this.potionSprite;
                } else {
                    this.itemRenderer.sprite = this.swordSprite;
                }
            } else {
                this.itemContainer.gameObject.SetActive(false);
                this.spriteRenderer.gameObject.SetActive(this._thoughtBubbleAnimation != ThoughtBubbleAnimation.None);
                this.spriteRenderer.sprite = null;
                this.animator.SetInteger(EmotionIndex, (int) this._thoughtBubbleAnimation);
            }
        }

        [Button("Test")]
        public void Test() {
            this.thoughtBubbleAnimation = ThoughtBubbleAnimation.Happy;
        }
    }
}