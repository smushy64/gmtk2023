using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player {
    public class PlayerInput: MonoBehaviour {
        private MappedInput mappedInput;
        public Vector2 movementVector { private set; get; }
        public bool actionPressed { private set; get; }
        public bool actionPressedThisFrame { private set; get; }
        public bool actionReleasedThisFrame { private set; get; }

        private void Awake() {
            this.mappedInput = new MappedInput();
        }
        private void OnEnable() {
            this.mappedInput.Enable();
        }
        private void OnDisable() {
            this.mappedInput.Disable();
        }

        public void UpdateInput() {
            this.movementVector = this.mappedInput.Character.Movement.ReadValue<Vector2>();
            
            this.actionPressed = this.mappedInput.Character.Action.IsPressed();
            this.actionPressedThisFrame = this.mappedInput.Character.Action.WasPressedThisFrame();
            this.actionReleasedThisFrame = this.mappedInput.Character.Action.WasReleasedThisFrame();
        }
    }
}