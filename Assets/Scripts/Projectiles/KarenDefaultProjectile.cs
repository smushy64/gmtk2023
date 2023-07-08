using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Projectiles {
    public class KarenDefaultProjectile: BaseProjectile {
        [SerializeField] private float fastSpeed;
        [SerializeField] private float slowSpeed;
        [SerializeField] private float timeTilSlowSpeed;

        private Vector2 direction;

        public void SetUp(Vector2 spawnLocation, Vector2 direction) {
            this.transform.position = spawnLocation;
            this.direction = direction;
            this.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,-this.direction.Angle()));
        }


        protected override void Update() {
            base.Update();

            if (this.hasSpawned == false || this.isRecycling) {
                return;
            }

            float moveStep;
            if (this.projectileAliveTimer >= this.timeTilSlowSpeed) {
                moveStep = this.slowSpeed * Time.deltaTime;
            } else {
                moveStep = this.fastSpeed * Time.deltaTime;
            }

            this.transform.position = transform.position + (this.direction * moveStep).ToVector3();
        }
    }
}