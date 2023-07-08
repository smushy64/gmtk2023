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
    //actually I don't think we even need to recycle these mofos
    public class HeroRecycler: GameMechanic {
        [SerializeField] private BaseHero[] heroPrefabs;
        [SerializeField] private Transform[] entrances;
        [SerializeField] private Transform[] waitingSpots;

        private Dictionary<int, BaseHero> indexedPrefabs; // I hate this;

        private void Awake() {
            this.indexedPrefabs = new();
            foreach (var prefab in this.heroPrefabs) {
                this.indexedPrefabs[prefab.heroType.GetPrefabHashCode(prefab.heroVariantValue)] = prefab;
            }
        }

        public BaseHero Create(HeroType heroType, byte variant) {
            var index = heroType.GetPrefabHashCode(variant);
            var prefab = this.indexedPrefabs[index];

            var hero = prefab.Instantiate(this.transform);
            hero.runner = this.runner;
            
            return hero;
        }

        [Button("Test Spawn", EButtonEnableMode.Playmode)]
        public void TestSpawn() {
            var hero = this.Create(HeroType.Merlin, (byte)MerlinVariant.BlueRobe);
            
            hero.SetUp(Item.Potion, new Vector2(3,3), new Vector2(5,5));
        }
#if UNITY_EDITOR
        [Button("Update prefabs", EButtonEnableMode.Editor)]
        public void UpdatePrefabs() {
            this.heroPrefabs = Resources.LoadAll<BaseHero>("Prefabs/Heroes");
        }
#endif
        
    }
}