using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using Player;
using Runner;
using UnityEngine;

namespace Projectiles {
    public interface IProjectileRecycler {
        public void Recycle(BaseProjectile projectile);
    }

    public enum ProjectileDeath {
        None,
        RanOutOfTime,
        HitWall, // stubbing it out to think through the idea
        HitPlayer,
    }
    public class BaseProjectile: MonoBehaviour, IPlayerCollisionDetectionListener {
        [SerializeField] protected float maxLifeOfProjectile = 10f;
        [SerializeField] protected float timeToRecycle = 0.1f;
        [SerializeField] protected int damage = 1;
        [SerializeField] protected PlayerCollisionDetection playerCollisionDetection;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        
        public IProjectileRecycler recycler;
        public GameRunner runner;
        
        protected bool hasSpawned = false;
        protected bool isRecycling = false;
        protected float projectileAliveTimer = 0f;

        protected ProjectileDeath projectileDeath;

        protected virtual void Awake() {
            this.playerCollisionDetection.listener = this;
        }

        protected virtual void Update() {
            if (this.hasSpawned == false) {
                return;
            }
            
            if (this.runner.status != GameRunner.Status.Running) {
                return;
            }
            this.projectileAliveTimer += Time.deltaTime;

            if (this.isRecycling) {
                return;
            }

            if (this.projectileAliveTimer >= this.maxLifeOfProjectile) {
                this.KillProjectile(ProjectileDeath.RanOutOfTime);
            }
        }

        public void OnRecycle() {
            this.spriteRenderer.DOKill();
            this.hasSpawned = false;
        }

        public virtual void OnSpawn() {
            this.projectileDeath = ProjectileDeath.None;
            this.hasSpawned = true;
            this.isRecycling = false;
            this.projectileAliveTimer = 0f;
        }

        protected async void KillProjectile(ProjectileDeath deathType) {
            this.isRecycling = true;
            this.projectileDeath = deathType;
            this.AnimateProjectileDeath();
            await UniTask.Delay(TimeSpan.FromSeconds(this.timeToRecycle));
            if (this == null) {
                return;
            }
            this.recycler.Recycle(this);
        }
        protected virtual void AnimateProjectileDeath() {
        }
        protected virtual void OnHitPlayer(PlayerController player, [CanBeNull] Collision2D collision) {
            if (this.hasSpawned == false || this.isRecycling) {
                return;
            }
            player.TakeDamage(this.damage);
            this.KillProjectile(ProjectileDeath.HitPlayer);
        }
        
        public void OnPlayerCollisionEnter2D(PlayerController player, PlayerCollisionDetection detection, Collision2D other) {
            this.OnHitPlayer(player, other);
        }

        public void OnPlayerTriggerEnter2D(PlayerController player, PlayerCollisionDetection detection, Collider2D other) {
            this.OnHitPlayer(player, null);
            
        }
    }
}