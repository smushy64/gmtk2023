using DG.Tweening;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Projectiles {
    public class GremlinBarfProjectile: BaseProjectile {
        [SerializeField] private Color barfColor;
        
        public override void OnSpawn() {
            base.OnSpawn();
            this.spriteRenderer.color = this.barfColor.WithAlpha(0f);
            
            this.spriteRenderer.DOColor(this.barfColor, 0.2f);
        }

        protected override void AnimateProjectileDeath() {
            base.AnimateProjectileDeath();
            this.spriteRenderer.DOColor(barfColor.WithAlpha(0f), 0.1f);
        }
    }
}