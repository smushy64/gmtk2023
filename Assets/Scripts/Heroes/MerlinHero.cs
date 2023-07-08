using Projectiles;
using UnityEngine;

namespace Heroes {
    public enum MerlinVariant: byte {
        BlueRobe
    }
    public class MerlinHero: BaseHero {
        public override HeroType heroType => HeroType.Merlin;
        public override byte heroVariantValue => (byte) this.variant;

        [SerializeField] private MerlinVariant variant;
        
        [SerializeField] private float attackSpeed = 0.5f;
        [SerializeField] private Transform attackFrom;


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


        private void Shoot() {
            var targeted = this.runner.projectileRecycler.Spawn<TargetedProjectile>();
            targeted.SetUp(this.attackFrom.position, this.runner.player.targetTransform.position);
        }
    }
}