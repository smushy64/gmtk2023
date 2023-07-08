using RamenSea.Foundation3D.Extensions;
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
            Vector3 movementDelta = this.input.movementVector * (this.movementSpeed * Time.deltaTime);
            this.transform.position += movementDelta;
        }

        public void TakeDamage(int damage) { // we could add source
            
        }
    }
}