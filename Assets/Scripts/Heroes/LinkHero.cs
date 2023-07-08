using Projectiles;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Heroes {
    public enum LinkVariant: byte {
        GreenTunic
    }
    public class LinkHero: BaseHero {
        public override HeroType heroType => HeroType.Link;
        public override byte heroVariantValue => (byte) this.variant;

        [SerializeField] private LinkVariant variant;
        [SerializeField] private float madWalkingSpeed;
        [SerializeField] private float targetingOffAxisDifference;


        private Vector2 chargeDirection;
        protected override void MoveState(BaseHeroState newState) {
            base.MoveState(newState);

            switch (this.state) {
                case BaseHeroState.Mad: {
                    break;
                }
            }
        }

        protected override void Update() {
            base.Update();

            switch (this.state) {
                case BaseHeroState.Mad: {
                    var directionToPlayer = this.transform.position.ToVector2()
                                                .Direction(this.runner.player.targetTransform.position);

                    if (directionToPlayer.x.Abs() <= this.targetingOffAxisDifference) {
                        this.StartCharge(new Vector2(0f,directionToPlayer.y >= 0 ? 1f : -1f));
                    } else if (directionToPlayer.y.Abs() <= this.targetingOffAxisDifference) {
                        this.StartCharge(new Vector2(directionToPlayer.x >= 0 ? 1f : -1f, 0f));
                    }
                    break;
                }
            }
        }

        private void StartCharge(Vector2 direction) {
            
        }
    }
}