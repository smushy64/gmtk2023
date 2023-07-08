using Items;
using Projectiles;
using RamenSea.Foundation.Extensions;
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
        [SerializeField] private float attackSpeed = 4f;
        [SerializeField] private Transform attackFrom;


        private float attackTimer = 0f;

        public override void SetUp(Item requestingItem, Vector2 spawnLocation, Vector2 requestLocation) {
            base.SetUp(Item.None, spawnLocation, requestLocation);
            this.requestsInteraction = true;
        }

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


        private void Shoot() {
            const int numShots = 5;
            const float angleStep = 360 / numShots;

            float angle = 0f;
            for (int i = 0; i < numShots; i++) {
                var targeted = this.runner.projectileRecycler.Spawn<KarenDefaultProjectile>();
                targeted.SetUp(this.attackFrom.position, angle.DegreeToDirection());
                angle += angleStep;
            }
        }

    }
}