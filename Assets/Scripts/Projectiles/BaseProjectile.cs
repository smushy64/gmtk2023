using System;
using UnityEngine;

namespace Projectiles {
    public interface IProjectileRecycler {
        public void Recycle(BaseProjectile projectile);
    }
    public class BaseProjectile: MonoBehaviour {
        public IProjectileRecycler recycler;


        protected void Update() {
            //stub this out
        }

        public void OnRecycle() {}
    }
}