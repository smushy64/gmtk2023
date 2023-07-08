using UnityEngine;

namespace Player {
    public class PlayerInput: MonoBehaviour {
        public Vector2 movementVector { private set; get; }
        public bool actionPressed { private set; get; }
        public bool actionPressedThisFrame { private set; get; }
        public bool actionReleasedThisFrame { private set; get; }

        public void UpdateInput() {
            // stub
            this.movementVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)); // lol
            this.actionPressed = false;
            this.actionPressedThisFrame = false;
            this.actionReleasedThisFrame = false;
        }
    }
}