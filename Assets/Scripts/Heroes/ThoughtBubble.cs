using System;
using NaughtyAttributes;
using UnityEngine;

namespace Heroes {

    public enum ThoughtBubbleAnimation {
        None, Mad, Happy
    }
    public class ThoughtBubble: MonoBehaviour {
        private static readonly int EmotionIndex = Animator.StringToHash("EmotionIndex");

        [SerializeField] private SpriteRenderer spriteRenderer;
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

        private void Awake() {
            this.spriteRenderer.gameObject.SetActive(false);
        }

        private void SetAnimation() {
            this.spriteRenderer.gameObject.SetActive(this._thoughtBubbleAnimation != ThoughtBubbleAnimation.None);
            this.spriteRenderer.sprite = null;
            this.animator.SetInteger(EmotionIndex, (int) this._thoughtBubbleAnimation);
        }

        [Button("Test")]
        public void Test() {
            this.thoughtBubbleAnimation = ThoughtBubbleAnimation.Happy;
        }
    }
}