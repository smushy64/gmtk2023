using System;
using Player;
using UnityEngine;

namespace Items {
    public class PotionBuilder: BaseItemBuilder {
        public override Item itemToBuild => Item.Potion;

        [SerializeField] private float timeItTakesToCook = 1f;
        [SerializeField] private float timeTilOverCooked = 1.5f;
        [SerializeField] private float timeItTakesToReset = 2f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private bool isCooking = false;
        private bool isReseting = false;

        private float cookingTime = 0f;
        private float resetTimer = 0f;
        private void Update() {
            if (this.isCooking) {
                this.cookingTime += Time.deltaTime;
                this.progressBar.SetProgress(this.cookingTime / this.timeItTakesToCook);

                if (this.cookingTime >= this.timeItTakesToCook) {
                    this.itemIsFinished = true;
                }
                if (this.cookingTime >= this.timeTilOverCooked) {
                    this.SetReset();
                }
            }

            if (this.isReseting) {
                this.resetTimer += Time.deltaTime;
                if (this.resetTimer >= this.timeItTakesToReset) {
                    this.isReseting = false;
                    this.spriteRenderer.color = Color.red;
                }
            }
        }

        private void SetReset() {
            this.resetTimer = 0f;
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