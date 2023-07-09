using System;
using DG.Tweening;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Projectiles {
    public class KarenDefaultProjectile: BaseProjectile {
        [SerializeField] private float fastSpeed;
        [SerializeField] private float slowSpeed;
        [SerializeField] private float timeTilSlowSpeed;
        [SerializeField] private float startScale;
        [SerializeField] private float endScale;
        [SerializeField] private Sprite[] possibleSprites;
        [SerializeField] private SpriteRenderer backgroundRenderer;

        private Vector2 direction;
        private System.Random random;

        protected override void Awake() {
            base.Awake();
            this.random = new();
        }

        public void SetUp(Vector2 spawnLocation, Vector2 direction) {
            this.transform.position = spawnLocation;
            this.direction = direction;
            this.spriteRenderer.transform.localScale = new Vector3(this.startScale, this.startScale, this.startScale);
            this.spriteRenderer.transform.DOScale(this.endScale, this.timeTilSlowSpeed);
            // this.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,-this.direction.Angle()));
            // this.spriteRenderer.sprite = this.possibleSprites.RandomElement(this.random);
            // this.backgroundRenderer.sprite = this.spriteRenderer.sprite;
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

        private void OnDestroy() {
            this.spriteRenderer.transform.DOKill();
        }
    }
}