using System;
using Cysharp.Threading.Tasks;
using Projectiles;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Heroes {
    public enum GremlinVariant: byte {
        Green
    }
    public class GremlinHero: BaseHero {
        private enum MadBehavior {
            Idle,
            WalkTowardsTarget,
            WalkTowardsTargetFast,
            WalkRandomly,
            Barf,
        }
        public override HeroType heroType => HeroType.Gremlin;
        public override byte heroVariantValue => (byte) this.variant;

        [SerializeField] private GremlinVariant variant;
        [SerializeField] private float randomWalkingSpeed;
        [SerializeField] private float walkingSpeedAtPlayer;
        [SerializeField] private float walkingSpeedAtPlayerFast;
        [SerializeField] private float waitToSpawnBarf;

        [SerializeField] private Transform attackFrom;


        private MadBehavior behavior;
        private float durationOfBehavior;
        private Vector2 randomDirection;
        private System.Random random;
        private bool firstTimeWithBehavior = false;
        
        
        protected override void Awake() {
            base.Awake();
            this.random = new System.Random();
        }

        protected override void MoveState(BaseHeroState newState) {
            base.MoveState(newState);

            switch (this.state) {
                case BaseHeroState.WalkingIn: {
                    break;
                }
                case BaseHeroState.Mad: {
                    this.SetNewBehavior();
                    break;
                }
            }
        }

        protected override void Update() {
            base.Update();

            switch (this.state) {
                case BaseHeroState.Mad: {
                    if (this.behavior != MadBehavior.WalkRandomly && this.behavior != MadBehavior.WalkTowardsTarget) {
                        this.HandleBehavior(Time.deltaTime);
                    }
                    break;
                }
            }
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();

            switch (this.state) {
                case BaseHeroState.Mad: {
                    if (this.behavior == MadBehavior.WalkRandomly || this.behavior == MadBehavior.WalkTowardsTarget) {
                        this.HandleBehavior(Time.fixedDeltaTime);
                    }
                    break;
                }
            }
        }

        private void HandleBehavior(float deltaTime) {
            this.durationOfBehavior -= deltaTime; // don't bother with overflow time
            
            switch (this.behavior) {
                case MadBehavior.Idle: {
                    if (this.firstTimeWithBehavior) {
                        this.SetMovementAnimation(Vector2.zero);
                    }
                    break;
                }
                case MadBehavior.Barf: {
                    if (this.firstTimeWithBehavior) {
                        this.SetMovementAnimation(Vector2.zero);
                        this.Barf();
                    }
                    break;
                }
                case MadBehavior.WalkRandomly: {
                    if (this.firstTimeWithBehavior) {
                        this.randomDirection = new Vector2(this.random.Next(-1f, 1f), this.random.Next(-1f, 1f));
                    }
                    Vector2 movementDelta = this.randomDirection * (this.randomWalkingSpeed * Time.fixedDeltaTime);
                    this.r2d.MovePosition(this.transform.position.ToVector2() + movementDelta);
                    this.SetMovementAnimation(movementDelta);
                    break;
                }
                case MadBehavior.WalkTowardsTarget:
                case MadBehavior.WalkTowardsTargetFast: {
                    var directionToPlayer = this.transform.position.ToVector2().Direction(this.runner.player.targetTransform.position);
                    var speed = this.behavior == MadBehavior.WalkTowardsTarget
                        ? this.walkingSpeedAtPlayer
                        : this.walkingSpeedAtPlayerFast;
                    Vector2 movementDelta = directionToPlayer * (speed * Time.fixedDeltaTime);
                    this.r2d.MovePosition(this.transform.position.ToVector2() + movementDelta);
                    this.SetMovementAnimation(movementDelta);
                    break;
                }
            }

            this.firstTimeWithBehavior = false;
            if (this.durationOfBehavior <= 0f) {
                this.SetNewBehavior();
            }
        }

        private void SetNewBehavior() {
            this.firstTimeWithBehavior = true;
            this.behavior = this.GetNextBehavior();
            this.durationOfBehavior = this.GetNextBehaviorTime(this.behavior);
        }

        private MadBehavior GetNextBehavior() { // allows for easier variant configuration
            if (this.behavior != MadBehavior.Barf && this.random.NextBool(this.GetBehaviorChance(MadBehavior.Barf))) {
                return MadBehavior.Barf;
            }

            if (this.random.NextBool(this.GetBehaviorChance(MadBehavior.WalkTowardsTarget))) {
                return MadBehavior.WalkTowardsTarget;
            }
            if (this.random.NextBool(this.GetBehaviorChance(MadBehavior.WalkRandomly))) {
                return MadBehavior.WalkRandomly;
            }

            return MadBehavior.Idle;
        }

        private float GetBehaviorChance(MadBehavior behavior) {
            switch (this.behavior) {
                case MadBehavior.Idle: {
                    return 0.33f;
                }
                case MadBehavior.WalkRandomly: {
                    return 0.33f;
                }
                case MadBehavior.WalkTowardsTarget: {
                    return 0.33f;
                }
                case MadBehavior.Barf: {
                    return 0.05f;
                }
            }

            return 0f;
        }
        private float GetNextBehaviorTime(MadBehavior behaviour) {
            switch (this.behavior) {
                case MadBehavior.Idle: {
                    return this.random.Next(0.3f,0.9f);
                }
                case MadBehavior.WalkRandomly: {
                    return this.random.Next(0.3f,1.4f);
                }
                case MadBehavior.WalkTowardsTarget: {
                    return this.random.Next(0.3f,1.4f);
                }
                case MadBehavior.Barf: {
                    return 50f; // really long time since the barf mechanic breaks itself
                }
            }

            return 0f;
        }

        private async void Barf() {
            this.animator.SetBool(ANIMATOR_ATTACK, true);
            
            await UniTask.Delay(TimeSpan.FromSeconds(this.waitToSpawnBarf), DelayType.DeltaTime);
            if (this == null) {
                return;
            }
            
            var barf = this.runner.projectileRecycler.Spawn<GremlinBarfProjectile>();
            barf.transform.position = this.attackFrom.position;
            
            await UniTask.NextFrame();
            if (this == null) {
                return;
            }
            this.animator.SetBool(ANIMATOR_ATTACK, false);
            this.SetNewBehavior();
        }
    }
}