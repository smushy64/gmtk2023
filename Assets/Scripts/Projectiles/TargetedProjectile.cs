using System;
using DG.Tweening;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Projectiles {
    public class TargetedProjectile: BaseProjectile {

        [SerializeField] private float maxSpeed;
        [SerializeField] private float timeTilMaxSpeed;
        [SerializeField] private float startDelay;
        [SerializeField] private float scaleFrom;
        [SerializeField] private float scaleTo;
        [SerializeField] private AnimationCurve speedCurve;

        private Vector2 direction;

        public void SetUp(Vector2 spawnLocation, Vector2 target) {
            this.transform.position = spawnLocation;
            this.direction = spawnLocation.Direction(target);
            this.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,-this.direction.Angle()));

            this.spriteRenderer.transform.localScale = new Vector3(this.scaleFrom, this.scaleFrom, this.scaleFrom);
            this.spriteRenderer.transform.DOScale(this.scaleTo, this.startDelay);
        }


        protected override void Update() {
            base.Update();

            if (this.hasSpawned == false || this.isRecycling) {
                return;
            }

            if (this.projectileAliveTimer < this.startDelay) {
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

        private void OnDestroy() {
            this.spriteRenderer.transform.DOKill();
        }
    }
}