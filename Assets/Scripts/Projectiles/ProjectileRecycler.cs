using System;
using System.Collections.Generic;
using NaughtyAttributes;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using Runner;
using UnityEngine;

namespace Projectiles {
    public class ProjectileRecycler: GameMechanic, IProjectileRecycler {

        [SerializeField] private BaseProjectile[] projectilePrefabs;

        private Dictionary<Type, BaseProjectile> indexedPrefabs;
        private Dictionary<Type, Stack<BaseProjectile>> recycledProjectiles; //maybe move indexing powers based off of an enum vs type. eeeh

        private void Awake() {
            this.indexedPrefabs = new();
            this.recycledProjectiles = new();
            
            foreach (var prefab in this.projectilePrefabs) {
                this.indexedPrefabs[prefab.GetType()] = prefab;
            }
        }

        public T Spawn<T>() where T : BaseProjectile {
            var projectileType = typeof(T);
            var recycled = this.recycledProjectiles.GetNullable(projectileType);

            T projectile;
            if (recycled != null && recycled.Count > 0) {
                projectile = (T) recycled.Pop();
                projectile.gameObject.SetActive(true);
            } else {
                projectile = (T) this.indexedPrefabs[projectileType].Instantiate(this.transform);
                projectile.recycler = this;
                projectile.runner = this.runner;
            }
            projectile.OnSpawn();
            
            return projectile;
        }

        public void Recycle(BaseProjectile projectile) {
            projectile.OnRecycle();
            projectile.gameObject.SetActive(false);
            
            var type = projectile.GetType();
            var recycled = this.recycledProjectiles.GetNullable(type);
            if (recycled == null) {
                recycled = new();
                this.recycledProjectiles[type] = recycled;
            }

            recycled.Push(projectile);
        }
#if UNITY_EDITOR
        [Button("Update prefabs")]
        public void UpdatePrefabs() {
            this.projectilePrefabs = Resources.LoadAll<BaseProjectile>("Prefabs/Projectiles");
        }
#endif
    }
}