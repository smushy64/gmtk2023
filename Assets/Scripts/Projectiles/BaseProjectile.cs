using UnityEngine;

namespace Projectiles {
    public interface IProjectileRecycler {
        public void Recycle(BaseProjectile projectile);
    }
    public class BaseProjectile: MonoBehaviour {
        public IProjectileRecycler recycler;
        
        public void OnRecycle() {}
    }
}