using Projectiles;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Components.Audio;
using RamenSea.Foundation3D.Extensions;
using Runner;
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
        [SerializeField] private float madChargingSpeed;
        [SerializeField] private float timeForBeginChargingAnimation;
        [SerializeField] private float durationOfCharge;
        [SerializeField] private float targetingOffAxisDifference;
        [SerializeField] private float distanceTriggerCharge;
        [SerializeField] private float chargeAttackRange;
        [SerializeField] private int chargeDamage;
        [SerializeField] private VariationAudioSource attackSound;


        private Vector2 chargeDirection;
        private float beginChargingAnimationTimer = 0f;
        private float chargetimer = 0f;
        private bool isCharging = false;
        
        protected override void MoveState(BaseHeroState newState) {
            base.MoveState(newState);

            switch (this.state) {
                case BaseHeroState.Mad: {
                    this.isCharging = false;
                    break;
                }
            }
        }

        protected override void FixedUpdate() {
            base.Update();
            if (this.runner.status != GameRunner.Status.Running) {
                return;
            }

            switch (this.state) {
                case BaseHeroState.Mad: {
                    if (this.isCharging == false) {
                        var distance = this.transform.position.ToVector2() - this.runner.player.targetTransform.position.ToVector2();

                        if (distance.x.Abs() <= this.targetingOffAxisDifference) {
                            this.StartCharge(new Vector2(0f,distance.y >= 0 ? -1f : 1f));
                        } else if (distance.y.Abs() <= this.targetingOffAxisDifference) {
                            this.StartCharge(new Vector2(distance.x >= 0 ? -1f : 1f, 0f));
                        }  else if (distance.magnitude <= this.distanceTriggerCharge) {
                            this.StartCharge(this.transform.position.ToVector2().Direction(this.runner.player.targetTransform.position));
                        } else {
                            Vector2 moveDirection = Vector2.zero;
                            if (distance.x.Abs() < distance.y.Abs()) {
                                moveDirection.x = distance.x > 0 ? -1f : 1f;
                            } else {
                                moveDirection.y = distance.y > 0 ? -1f : 1f;
                            }
                            var moveDelta = moveDirection * (this.madWalkingSpeed * Time.fixedDeltaTime);

                            this.SetMovementAnimation(moveDelta);
                            this.r2d.MovePosition(this.transform.position + moveDelta.ToVector3());
                        }
                        
                    } else {
                        if (this.beginChargingAnimationTimer > 0f) {
                            this.beginChargingAnimationTimer -= Time.fixedDeltaTime;
                            if (this.beginChargingAnimationTimer <= 0) {
                                this.attackSound.Play();
                            }
                        } else {
                            var moveDelta = this.chargeDirection * (this.madChargingSpeed * Time.fixedDeltaTime);
                            var animationDelta = this.chargeDirection;
                            if (animationDelta.y.Abs() > 0.001f) {
                                if (animationDelta.x.Abs() < 0.001f) {
                                    animationDelta.x = -1f;
                                }
                                animationDelta.y = 0;
                            }
                            
                            this.SetMovementAnimation(animationDelta);
                            this.r2d.MovePosition(this.transform.position + moveDelta.ToVector3());
                            this.chargetimer -= Time.fixedDeltaTime;
                            var distance = this.transform.position.ToVector2()
                                               .Distance(this.runner.player.targetTransform.position);
                            if (distance <= this.chargeAttackRange) { //// eh just measure the distance each frome LOL
                                this.runner.player.TakeDamage(this.chargeDamage);
                            }
                            
                            if (this.chargetimer <= 0f) {
                                this.isCharging = false;
                                this.animator.SetBool(ANIMATOR_ATTACK, false);
                            }

                        }
                    }
                    break;
                }
            }
        }

        private void StartCharge(Vector2 direction) {
            this.SetMovementAnimation(Vector2.zero);
            this.animator.SetBool(ANIMATOR_ATTACK, true);
            this.isCharging = true;
            this.beginChargingAnimationTimer = this.timeForBeginChargingAnimation;
            this.chargetimer = this.durationOfCharge;
            this.chargeDirection = direction;
        }
    }
}