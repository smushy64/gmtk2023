using System;
using RamenSea.Foundation3D.Extensions;
using Runner;
using UnityEngine;

namespace Player {
    public class PlayerController: GameMechanic {
        [SerializeField] private PlayerInput input;
        [SerializeField] private float movementSpeed;
        [SerializeField] private Rigidbody2D rigidbody;


        private void Update() {
            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }
            
            this.input.UpdateInput();
        }

        private void FixedUpdate() {
            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }
            Vector2 movementDelta = this.input.movementVector * (this.movementSpeed * Time.fixedDeltaTime);
            this.rigidbody.MovePosition(this.transform.position.ToVector2() + movementDelta);
        }

        public void TakeDamage(int damage) { // we could add source
            
        }
    }
}