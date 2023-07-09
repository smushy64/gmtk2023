using System;
using Cysharp.Threading.Tasks;
using Projectiles;
using RamenSea.Foundation3D.Components.Audio;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Heroes {
    public enum MerlinVariant: byte {
        BlueRobe
    }
    public class MerlinHero: BaseHero {
        public override HeroType heroType => HeroType.Merlin;
        public override byte heroVariantValue => (byte) this.variant;

        [SerializeField] private MerlinVariant variant;
        
        [SerializeField] private float waitToSpawnProjectile1 = 0.2f;
        [SerializeField] private float waitToSpawnProjectile2 = 0.2f;
        [SerializeField] private float waitToSpawnProjectile3 = 0.2f;
        [SerializeField] private float attackSpeed = 0.5f;
        [SerializeField] private Transform attackFrom;
        [SerializeField] private VariationAudioSource attackSound;


        private float attackTimer = 0f;

        protected override void MoveState(BaseHeroState newState) {
            base.MoveState(newState);

            switch (this.state) {
                case BaseHeroState.Mad: {
                    this.attackTimer = this.attackSpeed;
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
            this.animator.SetBool(ANIMATOR_ATTACK, true);
            await UniTask.Delay(TimeSpan.FromSeconds(this.waitToSpawnProjectile1), DelayType.DeltaTime);
            if (this == null) {
                return;
            }
            var targeted = this.runner.projectileRecycler.Spawn<TargetedProjectile>();
            targeted.SetUp(this.attackFrom.position + new Vector3(-0.5f, 0f), this.runner.player.targetTransform.position);
            
            await UniTask.Delay(TimeSpan.FromSeconds(this.waitToSpawnProjectile2), DelayType.DeltaTime);
            if (this == null) {
                return;
            }
            targeted = this.runner.projectileRecycler.Spawn<TargetedProjectile>();
            targeted.SetUp(this.attackFrom.position + new Vector3(0, - 0.5f), this.runner.player.targetTransform.position);
            
            await UniTask.Delay(TimeSpan.FromSeconds(this.waitToSpawnProjectile3), DelayType.DeltaTime);
            if (this == null) {
                return;
            }
            this.attackSound.Play();
            targeted = this.runner.projectileRecycler.Spawn<TargetedProjectile>();
            targeted.SetUp(this.attackFrom.position + new Vector3(0.5f, 0f), this.runner.player.targetTransform.position);
            
            await UniTask.NextFrame();
            if (this == null) {
                return;
            }
            this.animator.SetBool(ANIMATOR_ATTACK, false);
        }
    }
}