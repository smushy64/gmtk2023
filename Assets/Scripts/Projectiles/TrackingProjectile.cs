using UnityEngine;

namespace Projectiles {
    public class TrackingProjectile: BaseProjectile {

        private Transform target;
        private bool hasSpawned = false;

        public Vector2 speed;
        public void Spawn(Transform target) {
            this.target = target;
            this.hasSpawned = true;
        }
    }
}