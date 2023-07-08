using System;
using Player;
using UnityEngine;

namespace Items {
    public class PotionBuilder: BaseItemBuilder {
        public override Item itemToBuild => Item.Potion;

        [SerializeField] private SpriteRenderer spriteRenderer;

        private bool isCooking = false;
        private bool isReseting = false;

        private float cookingTime = 0f;
        private float reset_timer = 0f;
        private void Update() {
            if (this.isCooking) {
                this.cookingTime += Time.deltaTime;
                this.progressBar.SetProgress(this.cookingTime / this.completionTime);

                if (this.cookingTime >= this.completionTime) {
                    this.itemIsFinished = true;
                }
                if (this.cookingTime >= this.failureTime) {
                    this.SetReset();
                }
            }

            if (this.isReseting) {
                this.reset_timer += Time.deltaTime;
                if (this.reset_timer >= this.resetTime) {
                    this.isReseting = false;
                    this.spriteRenderer.color = Color.red;
                }
            }
        }

        private void SetReset() {
            this.reset_timer = 0f;
            this.isReseting = true;
            this.isCooking = false;
            this.itemIsFinished = false;
            this.spriteRenderer.color = Color.black;
        }

        public override void ProcessInput( Player.Input input) {
            base.ProcessInput(input);

            if (this.isReseting) {
                return;
            }
            
            if( input.is_interact_pressed ) {
                TurnOffOnCooking();
            }
        }

        public override void TakeItem() {
            base.TakeItem();
            this.isCooking = false;
        }

        private void TurnOffOnCooking() {
            if (this.isCooking) {
                this.progressBar.gameObject.SetActive(false);
                if (this.itemIsFinished == false) {
                    this.SetReset();
                }
            } else {
                this.progressBar.gameObject.SetActive(true);
                this.progressBar.SetProgress(0f);
                this.cookingTime = 0f;
                this.isCooking = true;
            }
        }
    }
}