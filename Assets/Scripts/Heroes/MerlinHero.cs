using UnityEngine;

namespace Heroes {
    public enum MerlinVariant: byte {
        BlueRobe
    }
    public class MerlinHero: BaseHero {
        public override HeroType heroType => HeroType.Merlin;
        public override byte heroVariantValue => (byte) this.variant;

        [SerializeField] private MerlinVariant variant;
    }
}