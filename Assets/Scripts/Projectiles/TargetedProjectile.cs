using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Projectiles {
    public class TargetedProjectile: BaseProjectile {

        [SerializeField] private float maxSpeed;
        [SerializeField] private float timeTilMaxSpeed;
        [SerializeField] private AnimationCurve speedCurve;

        private Vector2 direction;

        public void SetUp(Vector2 spawnLocation, Vector2 target) {
            this.transform.position = spawnLocation;
            this.direction = spawnLocation.Direction(target);
            this.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,-this.direction.Angle()));
        }


        protected override void Update() {
            base.Update();

            if (this.hasSpawned == false || this.isRecycling) {
                return;
            }

            float moveStep;
            if (this.projectileAliveTimer >= this.timeTilMaxSpeed) {
                moveStep = this.maxSpeed * Time.deltaTime;
            } else {
                moveStep = this.speedCurve.Evaluate(this.projectileAliveTimer / this.timeTilMaxSpeed) * Time.deltaTime;
            }

            this.transform.position = transform.position + (this.direction * moveStep).ToVector3();
        }
    }
}