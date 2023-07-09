using System;
using System.Collections.Generic;
using Items;
using NaughtyAttributes;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using Runner;
using Unity.VisualScripting;
using UnityEngine;

namespace Heroes {
    public class HeroRecycler: GameMechanic {
        [SerializeField] private BaseHero[] heroPrefabs;
        [SerializeField] private Transform[] entrances;
        [SerializeField] private Transform[] waitingSpots;

        private Dictionary<int, BaseHero> indexedPrefabs; // I hate this;
        private Dictionary<int, Stack<BaseHero>> recycledHeroes; // I hate this;

        private void Awake() {
            this.indexedPrefabs = new();
            this.recycledHeroes = new();
            foreach (var prefab in this.heroPrefabs) {
                this.indexedPrefabs[prefab.heroType.GetPrefabHashCode(prefab.heroVariantValue)] = prefab;
            }
        }

        public BaseHero Spawn(HeroType heroType, byte variant) {
            var index = heroType.GetPrefabHashCode(variant);

            var recycled = this.recycledHeroes.GetNullable(index);
            BaseHero h;
            if (recycled != null && recycled.Count > 0) {
                h = recycled.Pop();
            } else {
                var prefab = this.indexedPrefabs[index];
                h = prefab.Instantiate(this.transform);
                h.runner = this.runner;
                h.recycler = this;
            }
            h.OnSpawn();
            
            return h;
        }

        public void Recycle(BaseHero hero) {
            hero.gameObject.SetActive(false);
            
            var index = hero.heroType.GetPrefabHashCode(hero.heroVariantValue);
            var recycled = this.recycledHeroes.GetNullable(index);
            if (recycled == null) {
                recycled = new();
                this.recycledHeroes[index] = recycled;
            }
            recycled.Push(hero);
        }
#if UNITY_EDITOR
        [Button("Update prefabs", EButtonEnableMode.Editor)]
        public void UpdatePrefabs() {
            this.heroPrefabs = Resources.LoadAll<BaseHero>("Prefabs/Heroes");
        }
#endif
        
    }
}