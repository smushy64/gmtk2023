using System;
using Items;
using JetBrains.Annotations;
using RamenSea.Foundation3D.Extensions;
using Runner;
using UnityEngine;

namespace Player {
    public class PlayerController: GameMechanic {
        [SerializeField] private PlayerInput input;
        [SerializeField] private float movementSpeed;
        [SerializeField] private Rigidbody2D rigidbody;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private GameObject potionInHand;

        public Transform targetTransform => _targetTransform;

        private BaseItemBuilder selectedBuilder;
        private bool isInteractingWithBuilder = false;

        private void Awake() {
            this.potionInHand.SetActive(false);
        }

        private void Update() {
            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }
            
            this.input.UpdateInput();
            if (selectedBuilder != null) {
                this.selectedBuilder.ProcessInput(this.input);
                if (this.selectedBuilder.itemIsFinished && this.input.actionPressedThisFrame) {
                    this.selectedBuilder.TakeItem();
                    this.potionInHand.SetActive(true);
                }
                this.isInteractingWithBuilder = this.selectedBuilder.isBeingInteractedWith;
            }
        }

        private void FixedUpdate() {
            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }

            if (this.isInteractingWithBuilder == false) {
                Vector2 movementDelta = this.input.movementVector * (this.movementSpeed * Time.fixedDeltaTime);
                this.rigidbody.MovePosition(this.transform.position.ToVector2() + movementDelta);
            }
        }

        public void SetSelectedItemBuilderArea(BaseItemBuilder builder) {
            this.selectedBuilder = builder;
        }
        public void ClearSelectedItemBuilderArea(BaseItemBuilder builder) {
            if (this.selectedBuilder == builder) {
                this.selectedBuilder = null;
            }
        }
        public void TakeDamage(int damage) { // we could add source
            
        }
    }
}