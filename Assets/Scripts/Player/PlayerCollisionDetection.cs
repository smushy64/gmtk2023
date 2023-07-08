using System;
using Runner;
using UnityEngine;

namespace Player {
    // Override the events you care about
    public interface IPlayerCollisionDetectionListener {
        public void OnPlayerCollisionEnter2D(PlayerController player, PlayerCollisionDetection detection, Collision2D other) { } // Pass in the detector to allow listeners to differentiate easier
        public void OnPlayerCollisionExit2D(PlayerController player, PlayerCollisionDetection detection, Collision2D other) { }
        public void OnPlayerTriggerEnter2D(PlayerController player, PlayerCollisionDetection detection, Collider2D other) { }
        public void OnPlayerTriggerExit2D(PlayerController player, PlayerCollisionDetection detection, Collider2D other) { }
    }
    
    // Common class for detecting player collisions
    // This could use Unity Events, but IMO just having it be a basic listener in this case is simpler
    public class PlayerCollisionDetection: MonoBehaviour {
        public IPlayerCollisionDetectionListener listener;
        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.CompareTag(GameTags.PLAYER_TAG)) {
                var playerCollider = other.gameObject.GetComponent<PlayerCollider>();
                this.listener?.OnPlayerCollisionEnter2D(playerCollider.player, this, other);
            }
        }

        private void OnCollisionExit2D(Collision2D other) {
            if (other.gameObject.CompareTag(GameTags.PLAYER_TAG)) {
                var playerCollider = other.gameObject.GetComponent<PlayerCollider>();
                this.listener?.OnPlayerCollisionExit2D(playerCollider.player, this, other);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag(GameTags.PLAYER_TAG)) {
                var playerCollider = other.gameObject.GetComponent<PlayerCollider>();
                this.listener?.OnPlayerTriggerEnter2D(playerCollider.player, this, other);
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.gameObject.CompareTag(GameTags.PLAYER_TAG)) {
                var playerCollider = other.gameObject.GetComponent<PlayerCollider>();
                this.listener?.OnPlayerTriggerExit2D(playerCollider.player, this, other);
            }
        }
    }
}