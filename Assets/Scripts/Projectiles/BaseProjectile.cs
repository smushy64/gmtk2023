using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Player;
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
        public IProjectileRecycler recycler;

        protected bool hasSpawned = false;
        protected bool isRecycling = false;
        protected float projectileLifeLeft = 0f;

        protected ProjectileDeath projectileDeath;
        protected void Update() {
            if (this.hasSpawned == false || this.isRecycling) {
                return;
            }

            this.projectileLifeLeft -= Time.deltaTime;
            if (this.projectileLifeLeft <= 0) {
                this.KillProjectile(ProjectileDeath.RanOutOfTime);
            }
        }

        public void OnRecycle() {
            this.hasSpawned = false;
        }

        public virtual void OnSpawn() {
            this.projectileDeath = ProjectileDeath.None;
            this.hasSpawned = true;
            this.isRecycling = false;
            this.projectileLifeLeft = this.maxLifeOfProjectile;
        }

        protected async void KillProjectile(ProjectileDeath deathType) {
            this.isRecycling = true;
            this.projectileDeath = deathType;
            this.AnimateProjectileDeath();
            await UniTask.Delay(TimeSpan.FromSeconds(this.timeToRecycle));
            this.recycler.Recycle(this);
        }
        protected void AnimateProjectileDeath() {
            
        }
        protected virtual void OnHitPlayer(PlayerController player, [CanBeNull] Collision2D collision) {
            if (this.hasSpawned == false || this.isRecycling) {
                return;
            }
            player.TakeDamage(this.damage);
            this.KillProjectile(ProjectileDeath.HitPlayer);
        }
        
        public void OnPlayerCollisionEnter2D(PlayerController player, Collision2D other) {
            this.OnHitPlayer(player, other);
        }

        public void OnPlayerTriggerEnter2D(PlayerController player, Collider2D other) {
            this.OnHitPlayer(player, null);
            
        }
    }
}