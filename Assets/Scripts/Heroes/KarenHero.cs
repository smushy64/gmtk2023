using System;
using Cysharp.Threading.Tasks;
using Items;
using Projectiles;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Components.Audio;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Heroes {
    public enum KarenVariant: byte {
        Default
    }
    public class KarenHero: BaseHero {
        public override HeroType heroType => HeroType.Karen;
        public override byte heroVariantValue => (byte) this.variant;

        [SerializeField] private KarenVariant variant;
        [SerializeField] private float waitForAttackAnimation = 0.2f;
        [SerializeField] private float attackSpeed = 4f;
        [SerializeField] private Transform attackFrom;
        [SerializeField] protected VariationAudioSource attackSound;


        private float attackTimer = 0f;

        protected override void MoveState(BaseHeroState newState) {
            base.MoveState(newState);

            switch (this.state) {
                case BaseHeroState.Mad: {
                    this.attackTimer = 0.1f;
                    break;
                }
            }
        }

        protected override void Update() {
            base.Update();

            switch (this.state) {
                case BaseHeroState.Mad: {
                    this.attackTimer -= Time.deltaTime;
                    if (this.attackTimer <= 0f) {
                        this.attackTimer = this.attackSpeed;
                        this.Shoot();
                    }
                    break;
                }
            }
        }


        private async void Shoot() {
            const int numShots = 5;
            const float angleStep = 360 / numShots;

            this.animator.SetBool(ANIMATOR_ATTACK, true);
            await UniTask.Delay(TimeSpan.FromSeconds(this.waitForAttackAnimation), DelayType.DeltaTime);
            if (this == null) {
                return;
            }
            this.attackSound.Play();
            float angle = 0f;
            for (int i = 0; i < numShots; i++) {
                var targeted = this.runner.projectileRecycler.Spawn<KarenDefaultProjectile>();
                targeted.SetUp(this.attackFrom.position, angle.DegreeToDirection());
                angle += angleStep;
            }

            await UniTask.NextFrame();
            if (this == null) {
                return;
            }
            this.animator.SetBool(ANIMATOR_ATTACK, false);
        }

    }
}