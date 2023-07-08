using System;
using General;
using Player;
using Runner;
using UnityEngine;

namespace Items {
    public class BaseItemBuilder: GameMechanic, IPlayerCollisionDetectionListener {
        [SerializeField] private PlayerCollisionDetection playerCollisionDetection;
        [SerializeField] protected GameProgressBar progressBar;
        public virtual Item itemToBuild => Item.None;
        public bool isBeingInteractedWith { protected set; get; }
        public bool itemIsFinished { protected set; get; }
        protected bool playerIsWithinBounds = false;

        private void Awake() {
            this.playerCollisionDetection.listener = this;
            this.progressBar.gameObject.SetActive(false);
        }

        public virtual void ProcessInput(PlayerInput input) {
            
        }
        public virtual void TakeItem() {
            this.itemIsFinished = false;
            this.isBeingInteractedWith = false;
        }
        public void OnPlayerTriggerEnter2D(PlayerController player, Collider2D other) {
            this.playerIsWithinBounds = true;
            this.runner.player.SetSelectedItemBuilderArea(this);
        }

        public void OnPlayerTriggerExit2D(PlayerController player, Collider2D other) {
            this.playerIsWithinBounds = false;
            this.runner.player.ClearSelectedItemBuilderArea(this);
        }
    }
}