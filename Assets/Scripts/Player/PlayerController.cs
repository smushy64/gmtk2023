using System;
using Runner;
using UnityEngine;

namespace Player {
    public class PlayerController: GameMechanic {
        [SerializeField] private PlayerInput input;
        [SerializeField] private float movementSpeed;


        private void Update() {
            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }
            
            this.input.UpdateInput();
            var movementDelta = this.input.movementVector * (this.movementSpeed * Time.deltaTime);
        }

        public void TakeDamage(int damage) { // we could add source
            
        }
    }
}