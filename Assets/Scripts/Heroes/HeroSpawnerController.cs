using System;
using Items;
using NaughtyAttributes;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using Runner;
using UnityEngine;
using Random = System.Random;

namespace Heroes {
    public class HeroSpawnerController: GameMechanic {
        [SerializeField] private float timeTilSpawnEasy = 10f;
        [SerializeField] private float timeTilSpawnHard = 10f;
        [SerializeField] private Vector2 spawnArea;
        [SerializeField] private Vector2 distanceSpawn;

        private float spawnTimer = 0f;
        private int numberOfSpawns = 0;
        private Random random;

        private void Awake() {
            this.random = new();
        }

        private void Update() {
            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }

            this.spawnTimer -= Time.deltaTime;
            if (this.spawnTimer <= 0f) {
                this.RandomSpawn();

                if (this.numberOfSpawns < 5) {
                    this.spawnTimer = this.timeTilSpawnEasy;
                } else {
                    this.spawnTimer = this.timeTilSpawnHard;
                }
            }
        }

        [Button("Test Spawn", EButtonEnableMode.Playmode)]
        private void RandomSpawn() {
            this.numberOfSpawns++;
            var h = this.runner.heroRecycler.Spawn(HeroType.Merlin, (byte) MerlinVariant.BlueRobe);
            Vector2 spawn;
            if (this.random.NextBool(0.3f)) {
                spawn.y = -this.spawnArea.y;
                spawn.x = this.random.Next(-this.spawnArea.x * 0.6f, this.spawnArea.x);
            } else if (this.random.NextBool(0.3f)) {
                spawn.y = this.spawnArea.y;
                spawn.x = this.random.Next(-this.spawnArea.x * 0.6f, this.spawnArea.x);
            } else {
                spawn.y = this.random.Next(-this.spawnArea.y, this.spawnArea.y);
                spawn.x = this.spawnArea.x;
            }

            var directionToCenter = spawn.Direction(Vector2.zero);
            var requestLocation = spawn + directionToCenter * this.random.Next(this.distanceSpawn.x, this.distanceSpawn.y);
            h.SetUp(Item.Potion, spawn, requestLocation);
        }
        public override void OnStateChange(GameState state) {
            base.OnStateChange(state);

            if (state.status == GameState.Status.Running) {
                this.spawnTimer = 0.1f;
            }
        }
    }
}