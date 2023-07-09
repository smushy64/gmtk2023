using System;
using UnityEngine;

namespace Player {
    public class CameraController: MonoBehaviour {
        [SerializeField] private Transform trackingTransform;
        [SerializeField] private Vector2 maxDistanceToMove;
        [SerializeField] private Vector3 center;


        private void Update() {
            var playerOffset = this.trackingTransform.position - this.center;

            if (playerOffset.x > this.maxDistanceToMove.x) {
                playerOffset.x = this.maxDistanceToMove.x;
            } else if (playerOffset.x < -this.maxDistanceToMove.x) {
                playerOffset.x = -this.maxDistanceToMove.x;
            }
            if (playerOffset.y > this.maxDistanceToMove.y) {
                playerOffset.y = this.maxDistanceToMove.y;
            } else if (playerOffset.y < -this.maxDistanceToMove.y) {
                playerOffset.y = -this.maxDistanceToMove.y;
            }

            var moveTo = playerOffset;

            moveTo.z = this.center.z;
            this.transform.position = moveTo;
        }
    }
}