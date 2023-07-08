using System;
using System.Collections.Generic;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using Unity.VisualScripting;
using UnityEngine;

namespace Heroes {
    public class HeroRecycler: MonoBehaviour {
        [SerializeField] private BaseHero[] heroPrefabs;

        private Dictionary<int, BaseHero> indexedPrefabs; // I hate this;

        private void Awake() {
            this.indexedPrefabs = new();
            foreach (var prefab in this.heroPrefabs) {
                this.indexedPrefabs[prefab.heroType.GetPrefabHashCode(prefab.heroVariantValue)] = prefab;
            }
        }

        public BaseHero Spawn(HeroType heroType, byte variant) {
            var index = heroType.GetPrefabHashCode(variant);
            var prefab = this.indexedPrefabs[index];

            var hero = prefab.Instantiate(this.transform);

            return hero;
        }
        
        
        
    }
}