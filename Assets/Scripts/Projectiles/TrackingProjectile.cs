using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Projectiles {
    public class TrackingProjectile: BaseProjectile {

        [SerializeField] private float turnSpeed;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float timeTilMaxSpeed;
        [SerializeField] private AnimationCurve speedUpCurve;
        private Transform target;

        public Vector2 directionalSpeed;
        public void SetUp(Transform target) {
            this.target = target;
        }


        protected override void Update() {
            base.Update();

            if (this.hasSpawned == false || this.isRecycling) {
                return;
            }

            var directionToPlayer = this.transform.position.ToVector2().Direction(this.runner.player.targetTransform.position);
            var targetSpeed = this.directionalSpeed * this.maxSpeed;
            // var moveSpeed = 
        }
    }
}