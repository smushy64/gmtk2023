using System;
using System.Collections.Generic;
using Items;
using NaughtyAttributes;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using Runner;
using UnityEngine;
using Random = System.Random;

namespace Heroes {
    public class HeroSpawnerController: GameMechanic {
        [SerializeField] private BoxCollider2D door1SpawnArea;
        [SerializeField] private BoxCollider2D door1Entrance;
        [SerializeField] private BoxCollider2D door2SpawnArea;
        [SerializeField] private BoxCollider2D door2Entrance;
        [SerializeField] private BoxCollider2D door3SpawnArea;
        [SerializeField] private BoxCollider2D door3Entrance;
        [SerializeField] private Vector2 testSpawnArea;

        private float gameTimer = 0f;
        private Random random;

        private LevelData levelData = LevelData.Null;
        private HeroSpawn nextSpawn = HeroSpawn.Null;
        private int spawnIndex;

        public List<BaseHero> heroes;
        public Action<BaseHero> onHeroSpawn;
        
        private void Awake() {
            this.random = new();
        }

        private void Update() {
            if (this.levelData.isNull) {
                this.levelData = this.runner.levelController.GetLevelData();
                if (this.levelData.heroes.Length > 0) {
                    this.nextSpawn = this.levelData.heroes[0];
                }
            }
            if( runner.status != GameRunner.Status.Running ) {
                return;
            }

            this.gameTimer += Time.deltaTime;
            while (this.nextSpawn.isNull == false && this.nextSpawn.spawnTimeInSeconds <= this.gameTimer) {
                this.SpawnIn(this.nextSpawn);
                this.spawnIndex++;
                if (this.spawnIndex < this.levelData.heroes.Length) {
                    this.nextSpawn = this.levelData.heroes[this.spawnIndex];
                } else {
                    this.nextSpawn = HeroSpawn.Null;
                }
            }
        }

        private void SpawnIn(HeroSpawn spawn) {
            var h = this.runner.heroRecycler.Spawn(spawn.heroType, spawn.variant);
            var spawnArea = this.GetDoorSpawn(spawn.door);
            var entranceArea = this.GetDoorEntranceArea(spawn.door);
            var spawnPoint = new Vector2(
                x: this.random.Next(spawnArea.min.x, spawnArea.max.x),
                y: this.random.Next(spawnArea.min.y, spawnArea.max.y)
                );
            var walkToPoint = new Vector2(
                x: this.random.Next(entranceArea.min.x, entranceArea.max.x),
                y: this.random.Next(entranceArea.min.y, entranceArea.max.y)
                );
            h.SetUp(spawn.id, spawn.requestItem, spawnPoint, walkToPoint);
            this.heroes.Add(h);
            this.onHeroSpawn?.Invoke(h);
        }

        private Bounds GetDoorSpawn(SpawnDoor door) {
            switch (door) {
                case SpawnDoor.Door1:
                    return this.door1SpawnArea.bounds;
                case SpawnDoor.Door2:
                    return this.door2SpawnArea.bounds;
                case SpawnDoor.Door3:
                    return this.door3SpawnArea.bounds;
            }
            return this.door1SpawnArea.bounds;
        }
        private Bounds GetDoorEntranceArea(SpawnDoor door) {
            switch (door) {
                case SpawnDoor.Door1:
                    return this.door1Entrance.bounds;
                case SpawnDoor.Door2:
                    return this.door2Entrance.bounds;
                case SpawnDoor.Door3:
                    return this.door3Entrance.bounds;
            }
            return this.door1Entrance.bounds;
        }
        [Button("Test Spawn", EButtonEnableMode.Playmode)]
        private void RandomSpawn() {
            var h = this.runner.heroRecycler.Spawn(HeroType.Karen, 0);
            Vector2 spawn;
            if (this.random.NextBool(0.3f)) {
                spawn.y = -this.testSpawnArea.y;
                spawn.x = this.random.Next(-this.testSpawnArea.x * 0.6f, this.testSpawnArea.x);
            } else if (this.random.NextBool(0.3f)) {
                spawn.y = this.testSpawnArea.y;
                spawn.x = this.random.Next(-this.testSpawnArea.x * 0.6f, this.testSpawnArea.x);
            } else {
                spawn.y = this.random.Next(-this.testSpawnArea.y, this.testSpawnArea.y);
                spawn.x = this.testSpawnArea.x;
            }

            var directionToCenter = spawn.Direction(Vector2.zero);
            var requestLocation = spawn + directionToCenter * this.random.Next(2f, 3f);
            h.SetUp(-1, Item.Potion, spawn, requestLocation);
            
            this.heroes.Add(h);
            this.onHeroSpawn?.Invoke(h);
        }
    }
}